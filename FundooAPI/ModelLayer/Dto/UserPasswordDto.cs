using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class UserPasswordDto
{
    [Required(ErrorMessage = "Password required")]
    public string Password { get; set; }

    //[Required(ErrorMessage = "Token required")]
    //public string Token { get; set; }
}
