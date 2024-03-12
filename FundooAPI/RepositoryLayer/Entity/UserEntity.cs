using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity;

public class UserEntity
{
    public int UserId { get; set; }

    //[Required(ErrorMessage = "First name required")]
    public string FirstName { get; set; }

    //[Required(ErrorMessage = "Last name required")]
    public string LastName { get; set; }

    //[Required(ErrorMessage = "Email required")]
    public string Email { get; set; }

    //[Required(ErrorMessage = "Password required")]
    public string Password { get; set; }

}
