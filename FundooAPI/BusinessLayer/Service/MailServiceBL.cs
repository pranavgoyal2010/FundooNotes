using BusinessLayer.Interface;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class MailServiceBL : IMailServiceBL
{
    private readonly IMailServiceRL _mailServiceRL;

    public MailServiceBL(IMailServiceRL mailServiceRL)
    {
        _mailServiceRL = mailServiceRL;
    }

    public async Task<bool> SendEmail(string to, string subject, string htmlMessage)
    {
        return await _mailServiceRL.SendEmail(to, subject, htmlMessage);
    }
}
