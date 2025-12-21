using SocialAppBackend.Models;

namespace SocialAppBackend.Services;

public class PostsService
{
    private readonly List<Post> _posts = [];
    

    public IEnumerable<Post> GetAll() => _posts;

    public Post Add(Post post)
    {   
        // this isnt great as it doesn't consider deleting posts and race conditions but whatever im sure db would handle that anyway so ill leave it
        post.Id = _posts.Count + 1;
        post.CreatedAt = DateTime.UtcNow;
        _posts.Add(post);
        return post;

    }

    public int GetPostCount()
    {
        return this._posts.Count;
    }

    // first or default returns the first match or null
    public Post? GetPostById(int id) =>
    _posts.FirstOrDefault(post => post.Id == id);
}