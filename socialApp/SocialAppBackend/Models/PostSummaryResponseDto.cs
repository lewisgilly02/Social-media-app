using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

// this is the dto fo returning a bunch of post e.g getting your fyp - this will come without comments and the user will have to manually request (open the comments section)
// no real difference to be honest, the service just wont .include() the comments.
public class PostSummaryResponseDto
{
    public int Id { get; set; }

    
    public string AuthorId { get; set; } = string.Empty;


    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CommentResponseDto> Comments {get; set;} = new();

}