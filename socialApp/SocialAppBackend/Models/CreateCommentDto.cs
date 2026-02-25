
using System.ComponentModel.DataAnnotations;
public class CreateCommentDto
{   
    [Required]
    // I have yet to implement auth and i cant be bothered
    // coding a id generator so we will just accept it as input
    public int AuthorId {get; set;} 


    [Required]
    [MaxLength(280)]
    public string Content {get; set;} = "";
}