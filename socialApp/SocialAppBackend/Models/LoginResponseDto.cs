using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class LoginResponseDto
{
    [Required]
    public string Token {get; set;} = string.Empty;
    
}