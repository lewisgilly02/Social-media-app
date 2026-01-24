using SocialAppBackend.Data;
using SocialAppBackend.Models;
using Microsoft.EntityFrameworkCore;

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
    

    public async Task<Post> CreateAsync(string content)
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


    // first or default returns the first match or null
    public Task<Post?> GetByIdAsync(int id) =>
    _db.Posts.FindAsync(id).AsTask();



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