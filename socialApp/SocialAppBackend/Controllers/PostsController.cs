using Microsoft.AspNetCore.Mvc;
using SocialAppBackend.Models;
using SocialAppBackend.Common;
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

 

    [HttpGet("{id:int}")]
    [AllowAnonymous]

    public async Task<ActionResult<PostResponseDto>> GetById(int id)
    {
        var post = await _service.GetPostByIdAsync(id);

        _logger.LogInformation("post controller: client requested post id: {}, {}", id, Request.Path);

        if (!post.Success)
        {
            return post.Error switch
            {
                ServiceError.NotFound => NotFound(),
                _ => StatusCode(500)
            };

        }   
            // if post.success is true, data cant be null
            return post.Data!;
    }

    [HttpGet("{postid:int}/comments")]
    [AllowAnonymous]
    

    public async Task<ActionResult<List<CommentResponseDto>>> GetComments(int postId)
    {
        // this will check in service that the post exists but will return only the comments
        var comments = await _service.GetCommentsAsync(postId);

        if (!comments.Success)
        {
            return comments.Error switch
            {
                ServiceError.NotFound => NotFound(),
                _ => StatusCode(500)
            };
        }

        _logger.LogInformation("client is getting comments on post {}", postId);

        return Ok(comments.Data);
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

        return Ok(created.Data);
    }

    [HttpPost("{postId:int}/comments")]
    [Authorize]
    
    public async Task<ActionResult<CreateCommentDto>> CreateComment(int postId, [FromBody] CreateCommentDto dto)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var created = await _service.CreateCommentAsync(userId!, postId, dto.Content);

        if (!created.Success)
        {
            return created.Error switch
            {
                ServiceError.NotFound => NotFound(),
                _ => StatusCode(500)
            };    
        }

        _logger.LogInformation("client has made a comment on a post");
        return Ok(created.Data);
    }

    [HttpPost("{postId:int}/likes")]
    [Authorize]

    public async Task<ActionResult<LikeResponseDto>> CreateLike(int postId)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var liked = await _service.CreateLikeAsync(userId!, postId);

        if (!liked.Success)
        {
            return liked.Error switch
            {
                ServiceError.NotFound => NotFound(),
                ServiceError.Conflict => Conflict(),
                _ => StatusCode(500)
            };
        }

        _logger.LogInformation("user {} just liked post {}", userId, postId);

        return Ok(liked.Data);
    }


    //==================================== UDPATE
    [HttpPatch("{postid:int}")]
    [Authorize]

    public async Task<ActionResult<EditPostDto>> Edit(int postid, [FromBody] EditPostDto dto)
    {   

        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        
        var updatedPost = await _service.EditPost(userId!, postid, dto.content);

        if (!updatedPost.Success)
        {
            return updatedPost.Error switch
            {
                ServiceError.NotFound => NotFound(),
                ServiceError.Forbidden => Forbid(),
                _ => StatusCode(500)
            };
        }

        _logger.LogInformation("client edited post id: {}", postid);

        return Ok(updatedPost.Data);
    }

    // ================================== DELETE
    [HttpDelete("{id:int}/posts")]
    [Authorize]

    public async Task<ActionResult> DeletePost(int postId)
    {   
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var deleted = await _service.DeletePostAsync(userId!, postId);

        if (!deleted.Success)
        {
            return deleted.Error switch
            {
                ServiceError.NotFound => NotFound(),
                ServiceError.Forbidden => Forbid(),
                _ => StatusCode(500)
            };
        }
        _logger.LogInformation("client deleted post: {}", postId);

        return NoContent();
        
        
    }
    [HttpDelete("{postid:int}/likes")]
    [Authorize]

    public async Task<ActionResult> DeleteLike(int postid)
    {   

        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        
        var deleted = await _service.DeleteLikeAsync(userId!, postid);

        if (!deleted.Success)
        {
            return deleted.Error switch
            {
                ServiceError.NotFound => NotFound(),
                _ => StatusCode(500)
            };
        }

        _logger.LogInformation("user {} has unliked post {}", userId, postid);

        return NoContent();
    }

}