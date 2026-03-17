using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models.DTOs.Outbound;

public class LikeResponseDto
{
    [Required]
    public string UserId {get; set;} = "";

    [Required]

    public int PostId {get; set;}
}