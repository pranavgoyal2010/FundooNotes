namespace RepositoryLayer.Interface;

public interface IAuthService
{
    public string GenerateJwtToken(int UserId);
}
