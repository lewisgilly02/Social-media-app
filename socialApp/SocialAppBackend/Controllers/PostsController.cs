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

    public async Task<ActionResult<List<Post>>> GetAll()
    {
        var posts = await _service.GetAllAsync();
        return Ok(posts);
    }



    [HttpGet("{id:int}")]

    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        return post is null ? NotFound() : Ok(post);
    }



    [HttpGet("exception")]

    public void ForceException()
    {
        _service.ThrowAnException();
        
    }


    // posts
    
    [HttpPost]

    public async Task<ActionResult<Post>> Create([FromBody] CreatePostDto dto)
    {
        var created = await _service.CreateAsync(dto.Content);
        return Ok(created);
    }

    //update
    [HttpPatch]

    public async Task<ActionResult<Post>> Edit(int id, String content)
    {
        var updatedPost = await _service.EditPost(id, content);

        if (updatedPost is null) return NotFound();

        return Ok(updatedPost);
    }

    // delete

    [HttpDelete]

    public async Task<ActionResult<Post>> Delete(int id)
    {   

        var deleted = await _service.DeletePost(id);
        if (deleted is null) return NotFound();

        return NoContent();
        
        
    }

}