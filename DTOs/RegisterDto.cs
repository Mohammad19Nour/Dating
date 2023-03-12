using System.ComponentModel.DataAnnotations;

namespace Dating_App.DTOs;

public class RegisterDto
{
    [Required]
    public String UserName { get; set; }
    
    [Required]
    [StringLength(8,MinimumLength = 4)]
    public String Password { get; set; }
    [Required] public string KnownAs { get; set; }
    [Required] public string Gender { get; set; }
    [Required] public DateTime DateOfBirth { get; set; }
    [Required] public string City { get; set; }
    [Required] public string Country { get; set; }
}