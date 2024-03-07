using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface IUserRL
{
    public Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto);
}
