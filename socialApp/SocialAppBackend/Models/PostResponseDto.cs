using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

// this is the dto fo returning a singular post - this will come with comments pre loaded.
public class PostResponseDto
{
    public int Id { get; set; }

    
    public string AuthorId { get; set; } = string.Empty;


    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CommentResponseDto> Comments {get; set;} = new();

}