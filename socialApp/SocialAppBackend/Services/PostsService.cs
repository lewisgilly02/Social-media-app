using SocialAppBackend.Data;
using SocialAppBackend.Models;
using SocialAppBackend.Common;
using SocialAppBackend.Models.DTOs.Inbound;
using SocialAppBackend.Models.DTOs.Outbound;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;

namespace SocialAppBackend.Services;

public class PostsService
{
    
    private readonly AppDbContext _db;

    public PostsService(AppDbContext db)
    {
        _db = db;
    }


    public async Task<ServiceResult<PostResponseDto>> CreatePostAsync(string userId, string content)
    {
        var post = new Post
        {   
            AuthorId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };


        // by the time userid is here, it is null but userid comes from jwt so its in service
        _db.Posts.Add(post);

        await _db.SaveChangesAsync();

         PostResponseDto dto = new()
         {   
            AuthorId = userId,
            Id = post.Id,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            Comments = new()
        };

        return new ServiceResult<PostResponseDto> { Data = dto };
    }


    public async Task<ServiceResult<CreateCommentDto>> CreateCommentAsync(string authorId, int postId, string content)
    {   

        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId);

        if (!postExists) return new ServiceResult<CreateCommentDto>{Error = ServiceError.NotFound};

        var comment = new Comment
        {
            PostId = postId,
            AuthorId = authorId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);

        await _db.SaveChangesAsync();
        
        CreateCommentDto dto = new()
        {
            
            Content = comment.Content
        };

        return new ServiceResult<CreateCommentDto>{Data = dto};
    }

    public async Task<ServiceResult<LikeResponseDto>> CreateLikeAsync(string UserId, int postId)
    {   
        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId);

        if (!postExists) return new ServiceResult<LikeResponseDto>{Error = ServiceError.NotFound};


        var alreadyLiked = await _db.Likes
            .AnyAsync(l => l.PostId == postId && l.UserId == UserId);
        // when we expand error hadnling will have to give this the conflict error code 409
        
        if (alreadyLiked) return new ServiceResult<LikeResponseDto> { Error = ServiceError.Conflict };

        var liked = new Like()
        {
            PostId = postId,
            UserId = UserId
        };

        _db.Likes.Add(liked);

        await _db.SaveChangesAsync();

        LikeResponseDto dto = new()
        {
            PostId = postId,
            UserId = UserId
        };

        return new ServiceResult<LikeResponseDto>{Data = dto};
    }


    // first or default returns the first match or null
    public async Task<ServiceResult<PostResponseDto>> GetPostByIdAsync(int id)
    {   
        // for this, we must map this to a response dto to
        // omit the "post" field casuing infinite recursion
        var post = await _db.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(post => post.Id == id);
        
        if (post is null)
        {
            return new ServiceResult<PostResponseDto>{Error = ServiceError.NotFound};
        }

        PostResponseDto dto = new()
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            LikeCount = await _db.Likes.CountAsync(l => l.PostId == post.Id),
            Comments = post.Comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                PostId = c.PostId,
                AuthorId = c.AuthorId,
                Content = c.Content,
                CreatedAt = c.CreatedAt

            }).ToList()
                
            
        };

        return new ServiceResult<PostResponseDto>{Data = dto};
    }

    public async Task<ServiceResult<List<PostSummaryResponseDto>>> GetAllAsync()
        {
            // this function didn't work when enclosed with {}, find out why.
            // get all doesnt inlude comments
            var posts = await _db.Posts
                .OrderByDescending(post => post.CreatedAt)
                .Select(p => new PostSummaryResponseDto
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    Comments = new(),
                    LikeCount = p.Likes.Count()
                }).ToListAsync();

                return new ServiceResult<List<PostSummaryResponseDto>>{Data = posts}; 
    }

    public async  Task<ServiceResult<List<CommentResponseDto>>> GetCommentsAsync(int postId)
    {
        var post = await _db.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(post => post.Id == postId);
        
        if (post is null)
        {
            return new  ServiceResult<List<CommentResponseDto>>{Error = ServiceError.NotFound};
        }

        var comments = post.Comments.Select(c => new CommentResponseDto
        {
            Id = c.Id,
            PostId = c.PostId,
            AuthorId = c.AuthorId,
            Content = c.Content,
            CreatedAt = c.CreatedAt
        }).ToList();

        return new ServiceResult<List<CommentResponseDto>>{Data = comments};

    }


   


    public async Task<ServiceResult<PostResponseDto>> EditPost(string userId, int postId, String updatedContent)
    {
        var post = await _db.Posts
            .FindAsync(postId);

        if (post is null) return new ServiceResult<PostResponseDto>{Error = ServiceError.NotFound};

        if (post.AuthorId != userId) return new ServiceResult<PostResponseDto>{Error = ServiceError.Forbidden};

        post.Content = updatedContent;

        await _db.SaveChangesAsync();

        PostResponseDto dto = new()
        {
            Id = post.Id,

            AuthorId = post.AuthorId,

            Content = post.Content,

            CreatedAt = post.CreatedAt
        };

        return new ServiceResult<PostResponseDto>{Data = dto};
    }

    // doesnt need dto as controller wont return it anyway (returning whole post object may be wasteful tbf)
    // will look into some other time
    public async Task<ServiceResult<Post>> DeletePostAsync(string userId, int id)
    {
        var post = await _db.Posts.FindAsync(id);

        if (post is null) return new ServiceResult<Post>{Error = ServiceError.NotFound};

        if (post.AuthorId != userId) return new ServiceResult<Post>{Error = ServiceError.Forbidden};

        _db.Posts.Remove(post);

        await _db.SaveChangesAsync();
        
        return new ServiceResult<Post>{Data = post};
    }

    public async Task<ServiceResult<Like>> DeleteLikeAsync(string userId, int postId)
    {
        var like = await _db.Likes
            .FindAsync(userId, postId);
        
        if (like is null) return new ServiceResult<Like>{Error = ServiceError.NotFound};
        
        _db.Likes.Remove(like);

        await _db.SaveChangesAsync();

        return new ServiceResult<Like>{Data = like};
    }


}