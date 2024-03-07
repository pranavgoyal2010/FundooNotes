using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface IUserRL
{
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto);
    public Task<bool> LoginUser(UserLoginDto userLoginDto);
}
