// not used - just learning basics of middleware

namespace SocialApp.Middleware;

public class RequestLoggingMiddleware
{

    // request delegate is a function pointer that refers to the next piece in the middleware
    private readonly RequestDelegate _next;


    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    // all middleware will have invoke or invokeasync - this calls on each time the middleware is used.
    public async Task InvokeAsync(HttpContext context)
    {
        // do something before eg log request
        Console.WriteLine($"Request arrived: {context.Request.Method} - pipeline beginning work");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        // call next middleware - this will yield execution until the end of the pipeline
        await _next(context);

        // do something after e.g log response
        sw.Stop();
        Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");
    }
}