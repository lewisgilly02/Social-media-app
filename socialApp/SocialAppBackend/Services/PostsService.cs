using SocialAppBackend.Data;
using SocialAppBackend.Models;
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


    public async Task<PostResponseDto> CreatePostAsync(string content)
    {
        var post = new Post
        {
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _db.Posts.Add(post);

        await _db.SaveChangesAsync();

        return new PostResponseDto
        {
            Id = post.Id,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            Comments = new()
        };
    }


    public async Task<CreateCommentDto?> CreateCommentAsync(int postId, string authorId, string content)
    {   

        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId);

        if (!postExists) return null;

        var comment = new Comment
        {
            PostId = postId,
            AuthorId = authorId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);

        await _db.SaveChangesAsync();
        
        return new CreateCommentDto
        {
            AuthorId = comment.AuthorId,

            Content = comment.Content
        };
    }

    public async Task<LikeResponseDto?> CreateLikeAsync(int postId, string UserId)
    {   
        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId);

        if (!postExists) return null;


        var alreadyLiked = await _db.Likes
            .AnyAsync(l => l.PostId == postId && l.UserId == UserId);
        // when we expand error hadnling will have to give this the conflict error code 409
        if (alreadyLiked) return null;

        var liked = new Like()
        {
            PostId = postId,
            UserId = UserId
        };

        _db.Likes.Add(liked);

        await _db.SaveChangesAsync();

        return new LikeResponseDto
        {
            PostId = postId,
            UserId = UserId
        };
    }


    // first or default returns the first match or null
    public async Task<PostResponseDto?> GetPostByIdAsync(int id)
    {   
        // for this, we must map this to a response dto to
        // omit the "post" field casuing infinite recursion
        var post = await _db.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(post => post.Id == id);
        
        if (post is null)
        {
            return null;
        }

        return new PostResponseDto
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
    }

        public Task<List<PostSummaryResponseDto>> GetAllAsync()
    {
         // this function didn't work when enclosed with {}, find out why.
         // get all doesnt inlude comments
         var posts = _db.Posts
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

            return posts;
            
    
        
    }
   


 // TODO - change these to DTOs between now and this app's official deployment!
    public async Task<Post?> EditPost(int id, String updatedContent)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return null;

        post.Content = updatedContent;

        await _db.SaveChangesAsync();

        return post;
    }

    public async Task<Post?> DeletePostAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return null;

        _db.Posts.Remove(post);

        await _db.SaveChangesAsync();
        
        return post;
    }

    public async Task<Like?> DeleteLikeAsync(string userId, int postId)
    {
        var like = await _db.Likes
            .FindAsync(userId, postId);
        
        if (like is null) return null;
        
        _db.Likes.Remove(like);

        await _db.SaveChangesAsync();

        return like;
    }

    public void ThrowAnException()
    {
        throw new InvalidOperationException("Testing the global error handler - artificial error ");
    }
}