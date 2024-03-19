using BusinessLayer.Interface;
using ModelLayer.Dto;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class UserBL : IUserBL
{
    private readonly IUserRL _userRL;

    public UserBL(IUserRL userRL)
    {
        _userRL = userRL;
    }
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto)
    {
        return _userRL.RegisterUser(userRegistrationDto);
    }
    public Task<string> LoginUser(UserLoginDto userLoginDto)
    {
        return _userRL.LoginUser(userLoginDto);
    }
    public Task<string> ForgotPassword(string email)
    {
        return _userRL.ForgotPassword(email);
    }
    public Task<bool> ResetPassword(string newPassword, int userId)
    {
        return _userRL.ResetPassword(newPassword, userId);
    }
}
