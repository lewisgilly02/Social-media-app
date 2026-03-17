
using System.ComponentModel.DataAnnotations;
namespace SocialAppBackend.Models.DTOs.Inbound;
public class CreatePostDto
{   


    [Required]
    [MaxLength(280)]
    public string Content {get; set;} = "";
}