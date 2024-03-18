namespace RepositoryLayer.Interface;

public interface IMailServiceRL
{
    public Task<bool> SendEmail(string to, string subject, string htmlMessage);
}
