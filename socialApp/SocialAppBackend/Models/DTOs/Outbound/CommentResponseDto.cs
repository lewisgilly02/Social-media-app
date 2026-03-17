using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models.DTOs.Outbound;

/*
*  when we return a comment w/o the dto, it will infinite recurse
* because each comment has a post which has comments list and so on
* so we will omit the "post" field. This is best practice for any reponse
*/
public class CommentResponseDto
{
    
    public int Id { get; set; }

    [Required]
    //fk value
    public int PostId {get; set;}


    [Required]
    public string AuthorId { get; set; } = "";

    [Required]
    [MaxLength(250)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}