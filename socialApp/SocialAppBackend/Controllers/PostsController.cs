using Microsoft.AspNetCore.Mvc;
using SocialAppBackend.Models;
using SocialAppBackend.Services;
namespace SocialAppBackend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly PostsService _service;

    private readonly ILogger<PostsController> _logger;

    // dependency injection will fill this out
    public PostsController(PostsService service, ILogger<PostsController> log)
    {
        _service = service;
        _logger = log;
    }

    // get requests
    [HttpGet]
    public IEnumerable<Post> GetAll() => _service.GetAll();

    [HttpGet("{id}")]
    public ActionResult<Post> GetById(int id)
    {
        _logger.LogInformation($"Get request at /api/posts/(id) received for {id}");
        var post = _service.GetPostById(id);
        if (post is null)
        {
            _logger.LogWarning($"error post {id} cannot be found");
            return NotFound();
        }
        else
        {
            return Ok(post);
        }
    }

    [HttpGet("exception")]

    public void ForceException()
    {
        _service.ThrowAnException();
        
    }

    
    [HttpPost]

    public ActionResult<Post> Add(Post post)
    {

        var created = _service.Add(post);
        _logger.LogInformation($"post added, index: {_service.GetPostCount()} ");
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }


}