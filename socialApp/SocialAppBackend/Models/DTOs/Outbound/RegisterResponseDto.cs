using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models.DTOs.Outbound;

public class RegisterResponseDto
{
    [Required]

    public string UserName {get; set;} = string.Empty;

}