using Microsoft.AspNetCore.Mvc;
using SocialAppBackend.Models;
using SocialAppBackend.Models.DTOs.Inbound;
using SocialAppBackend.Models.DTOs.Outbound;
using SocialAppBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
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



    // =========================== GET / READ
    [HttpGet]
    [AllowAnonymous]

    public async Task<ActionResult<List<PostSummaryResponseDto>>> GetAll()
    {
        _logger.LogInformation("post controller: client requested all posts: {}", Request.Path);
        var posts = await _service.GetAllAsync();
        return Ok(posts);
    }

    // change to dtos

    [HttpGet("{id:int}")]
    [AllowAnonymous]

    public async Task<ActionResult<PostResponseDto>> GetById(int id)
    {
        var post = await _service.GetPostByIdAsync(id);
        _logger.LogInformation("post controller: client requested post id: {}, {}", id, Request.Path);
        return post is null ? NotFound() : Ok(post);
    }





    // ============================== CREATE
    [HttpPost]
    [Authorize]

    public async Task<ActionResult<CreatePostDto>> CreatePost([FromBody] CreatePostDto dto)
    {   
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        // [authorize] guarantees a valid token exists so sub wont be null
        var created = await _service.CreatePostAsync(userId!, dto.Content);
        _logger.LogInformation("client created a post");
        return Ok(created);
    }

    [HttpPost("{postId:int}/comments")]
    [Authorize]
    
    public async Task<ActionResult<CreateCommentDto>> CreateComment(int postId, [FromBody] CreateCommentDto dto)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var created = await _service.CreateCommentAsync(userId!, postId, dto.Content);

        if (created is null) return NotFound();

        _logger.LogInformation("client has made a comment on a post");
        return Ok(created);
    }

    [HttpPost("{postId:int}/likes")]
    [Authorize]

    public async Task<ActionResult<LikeResponseDto>> CreateLike(int postId)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var liked = await _service.CreateLikeAsync(userId!, postId);

        if (liked is null) return NotFound();

        _logger.LogInformation("user {} just liked post {}", userId, postId);

        return Ok(liked);
    }


    //==================================== UDPATE
    [HttpPatch("{id:int}")]
    [Authorize]

    public async Task<ActionResult<EditPostDto>> Edit(int postid, [FromBody] EditPostDto dto)
    {
        var updatedPost = await _service.EditPost(postid, dto.content);

        if (updatedPost is null) return NotFound();

        _logger.LogInformation("client edited post id: {}", postid);

        return Ok(updatedPost);
    }

    // ================================== DELETE
    [HttpDelete("{id:int}/posts")]
    [Authorize]

    public async Task<ActionResult> DeletePost(int id)
    {   

        var deleted = await _service.DeletePostAsync(id);
        if (deleted is null) return NotFound();
        _logger.LogInformation("client deleted post: {}", id);

        return NoContent();
        
        
    }
    [HttpDelete("{postid:int}/likes")]
    [Authorize]

    public async Task<ActionResult> DeleteLike(int postid)
    {   

        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        
        var deleted = await _service.DeleteLikeAsync(userId!, postid);

        if (deleted is null)
        {
            return Conflict();
        }

        _logger.LogInformation("user {} has unliked post {}", userId, postid);

        return NoContent();
    }

}