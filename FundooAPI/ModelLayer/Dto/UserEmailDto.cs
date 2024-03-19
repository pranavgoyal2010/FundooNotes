using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserEmailDto
{
    [Required(ErrorMessage = "Email required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
}
