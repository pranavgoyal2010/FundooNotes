using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface IUserBL
{
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto);
    public Task<string> LoginUser(UserLoginDto userLoginDto);
    public Task<string> ForgotPassword(string email);
    public Task<bool> ResetPassword(string newPassword, int userId);
}
