using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserLoginDto
{
    [Required(ErrorMessage = "Email required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password required")]
    public string Password { get; set; }

}
