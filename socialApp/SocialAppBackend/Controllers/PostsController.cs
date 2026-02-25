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
        _logger.LogInformation("post controller: client requested all posts: {}", Request.Path);
        var posts = await _service.GetAllAsync();
        return Ok(posts);
    }



    [HttpGet("{id:int}")]

    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await _service.GetPostByIdAsync(id);
        _logger.LogInformation("post controller: client requested post id: {}, {}", id, Request.Path);
        return post is null ? NotFound() : Ok(post);
    }



    [HttpGet("exception")]

    public void ForceException()
    {
        _service.ThrowAnException();
        
    }


    // create
    
    [HttpPost]

    public async Task<ActionResult<Post>> Create([FromBody] CreatePostDto dto)
    {
        var created = await _service.CreatePostAsync(dto.Content);
        _logger.LogInformation("client created a post");
        return Ok(created);
    }

    [HttpPost("{postId:int}/comments")]
    
    public async Task<ActionResult<Comment>> CreateComment(int postId, [FromBody] CreateCommentDto dto)
    {
        var created = await _service.CreateCommentAsync(postId, dto.AuthorId, dto.Content);

        if (created is null) return NotFound();

        _logger.LogInformation("client has made a comment on a post");
        return Ok(created);
    }

    //update
    [HttpPatch("{id:int}")]

    public async Task<ActionResult<Post>> Edit(int id, [FromBody] EditPostDto dto)
    {
        var updatedPost = await _service.EditPost(id, dto.content);

        if (updatedPost is null) return NotFound();

        _logger.LogInformation("client edited post id: {}", id);

        return Ok(updatedPost);
    }

    // delete
    [HttpDelete("{id:int}")]

    public async Task<ActionResult> Delete(int id)
    {   

        var deleted = await _service.DeletePost(id);
        if (deleted is null) return NotFound();
        _logger.LogInformation("client deleted post: {}", id);

        return NoContent();
        
        
    }

}