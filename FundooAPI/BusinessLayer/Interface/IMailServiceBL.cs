namespace BusinessLayer.Interface;

public interface IMailServiceBL
{
    public Task<bool> SendEmail(string to, string subject, string htmlMessage);
}
