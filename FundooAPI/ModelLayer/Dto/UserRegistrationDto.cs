using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserRegistrationDto
{
    [Required(ErrorMessage = "First name required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password required")]
    public string Password { get; set; }
}
