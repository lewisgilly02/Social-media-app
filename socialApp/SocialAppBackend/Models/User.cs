using System.ComponentModel.DataAnnotations;

namespace SocialAppBackend.Models;

public class User
{
    [Required]
    public string UserId {get; set;} = Guid.NewGuid().ToString();

    [Required]

    public string UserName {get; set;} = string.Empty;

    [Required]
    public string PasswordHash {get; set;} = string.Empty;
    
}