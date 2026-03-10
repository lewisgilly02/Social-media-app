using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class RegisterResponseDto
{
    [Required]

    public string UserName {get; set;} = string.Empty;

}