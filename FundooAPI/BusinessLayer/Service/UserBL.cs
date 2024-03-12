using BusinessLayer.Interface;
using ModelLayer.Dto;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class UserBL : IUserBL
{
    private readonly IUserRL _userRL;
    private readonly IAuthService _authService;
    public UserBL(IUserRL userRL, IAuthService authService)
    {
        _userRL = userRL;
        _authService = authService;
    }
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto)
    {
        return _userRL.RegisterUser(userRegistrationDto);
    }
    public Task<int> LoginUser(UserLoginDto userLoginDto)
    {
        return _userRL.LoginUser(userLoginDto);
    }

    public IAuthService authService()
    {
        return _authService;
    }
}
