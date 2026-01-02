namespace SocialApp.Middleware;

using System.Text.Json;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex ,"something went wrong on a request to {} {}", context.Request.Method, context.Request.Path);


            // this checks if the response has been written before - if it has and you proceed can get some weird bugs (not that it would come up here but future proof)
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                error = "something went wrong...",
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}