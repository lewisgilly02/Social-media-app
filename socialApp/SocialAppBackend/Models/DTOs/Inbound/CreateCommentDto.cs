
using System.ComponentModel.DataAnnotations;
namespace SocialAppBackend.Models.DTOs.Inbound;
public class CreateCommentDto
{   


    [Required]
    [MaxLength(280)]
    public string Content {get; set;} = "";
}