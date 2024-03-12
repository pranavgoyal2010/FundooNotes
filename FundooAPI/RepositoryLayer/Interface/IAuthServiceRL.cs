using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface IAuthServiceRL
{
    public string GenerateJwtToken(UserLoginDto userLoginDto);
}
