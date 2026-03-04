using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class Post
{
    public int Id { get; set; }

    
    public string AuthorId { get; set; } = "";


    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Comment> Comments {get; set;} = new();

}