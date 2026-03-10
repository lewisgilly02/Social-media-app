using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class LoginDto
{


    public string UserName {get; set;} = string.Empty;

    [Required]
    public string Password {get; set;} = string.Empty;
    
}