using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class Post
{
    public int Id { get; set; }

    
    public string Author { get; set; } = string.Empty;


    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}