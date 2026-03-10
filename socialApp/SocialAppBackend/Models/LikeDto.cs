using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class LikeDto
{
    [Required]
    public string UserId {get; set;} = "";

   // post id is not required in the dto as it will come from the path

}