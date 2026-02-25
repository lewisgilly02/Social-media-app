using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class Comment
{
    
    public int Id { get; set; }

    [Required]
    //fk value
    public int PostId {get; set;}

    [Required]
    public Post Post {get; set;} = null!;

    [Required]
    public int AuthorId { get; set; } 

    [Required]
    [MaxLength(250)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}