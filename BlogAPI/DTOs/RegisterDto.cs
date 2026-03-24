using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs;

public class RegisterDto
{
    [MaxLength(64)]
    public required string Email { get; set; }
    
    [MinLength(8)]
    public required string Password { get; set; }
}