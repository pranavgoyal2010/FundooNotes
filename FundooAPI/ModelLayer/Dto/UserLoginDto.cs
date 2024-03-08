using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserLoginDto
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }

}
