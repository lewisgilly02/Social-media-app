using System.ComponentModel.DataAnnotations;

public class EditPostDto
{
    [Required]
    [MaxLength(280)]
    public string content {get; set;} = "";
}