using System.ComponentModel.DataAnnotations;

namespace Dating_App.DTOs;

public class LoginDto
{
    [Required]
    public String UserName { get; set; }
    
    [Required]
    public String Password { get; set; }
}