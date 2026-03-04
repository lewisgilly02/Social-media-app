using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class Like
{
    [Required]
    public string UserId {get; set;} = "";

    [Required]

    public int PostId {get; set;}
}