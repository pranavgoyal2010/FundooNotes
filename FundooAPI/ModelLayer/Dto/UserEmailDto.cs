using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserEmailDto
{
    [Required(ErrorMessage = "Email required")]
    public string Email { get; set; }
}
