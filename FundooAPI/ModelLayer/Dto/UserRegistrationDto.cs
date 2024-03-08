using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserRegistrationDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}
