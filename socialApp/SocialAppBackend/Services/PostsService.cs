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

    public Task<List<Post>> GetAllAsync()
    // this function didn't work when enclosed with {}, find out why.
        =>  _db.Posts
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();
    

    public async Task<Post> CreatePostAsync(string content)
    {
        var post = new Post
        {
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _db.Posts.Add(post);

        await _db.SaveChangesAsync();

        return post;
    }

    public async Task<Comment> CreateCommentAsync(int postId, int authorId, string content)
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
        
        return comment;
    }


    // first or default returns the first match or null
    public async Task<Post?> GetPostByIdAsync(int id)
    {
        var post = await _db.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(post => post.Id == id);
        
        if (post is null)
        {
            return null;
        }

        return post;
    }



    public async Task<Post?> DeletePost(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return null;

        _db.Posts.Remove(post);

        await _db.SaveChangesAsync();
        
        return post;
    }



    public async Task<Post?> EditPost(int id, String updatedContent)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return null;

        post.Content = updatedContent;

        await _db.SaveChangesAsync();

        return post;
    }

    public void ThrowAnException()
    {
        throw new InvalidOperationException("Testing the global error handler - artificial error ");
    }
}