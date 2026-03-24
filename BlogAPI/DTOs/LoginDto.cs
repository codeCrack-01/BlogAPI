using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs;

public class LoginDto
{
    [MaxLength(64)]
    public required string Email { get; set; }
    public required string Password { get; set; }
}