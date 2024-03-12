using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface IUserBL
{
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto);
    public Task<string> LoginUser(UserLoginDto userLoginDto);
}
