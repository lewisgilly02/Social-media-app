using System.ComponentModel.DataAnnotations;
namespace SocialAppBackend.Models.DTOs.Inbound;
public class EditPostDto
{
    [Required]
    [MaxLength(280)]
    public string content {get; set;} = "";
}