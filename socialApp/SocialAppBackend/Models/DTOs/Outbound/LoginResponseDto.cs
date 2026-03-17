using System.ComponentModel.DataAnnotations;
namespace SocialAppBackend.Models.DTOs.Outbound;

public class LoginResponseDto
{
    [Required]
    public string Token {get; set;} = string.Empty;
    
}