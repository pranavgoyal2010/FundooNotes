using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface;

public interface IAuthServiceRL
{
    public string GenerateJwtToken(UserEntity userEntity);
}
