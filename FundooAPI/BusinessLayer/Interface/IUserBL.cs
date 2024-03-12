using ModelLayer.Dto;
using RepositoryLayer.Interface;

namespace BusinessLayer.Interface;

public interface IUserBL
{
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto);
    public Task<int> LoginUser(UserLoginDto userLoginDto);
    public IAuthService authService();// { get { return _authService; } }
}
