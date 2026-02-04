namespace LlamaEngineHost.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
public class LoginRequestDTO
{
    [Required]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int GroupNumber {get; set;}
}