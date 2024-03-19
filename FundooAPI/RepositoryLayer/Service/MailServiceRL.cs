using ModelLayer.Dto;
using RepositoryLayer.CustomException;
using RepositoryLayer.Interface;
using System.Net;
using System.Net.Mail;

namespace RepositoryLayer.Service;

public class MailServiceRL : IMailServiceRL
{
    private readonly EmailDto _emailDto;
    public MailServiceRL(EmailDto emailDto)
    {
        _emailDto = emailDto;
    }

    public async Task<bool> SendEmail(string to, string subject, string htmlMessage)
    {
        try
        {
            using (var client = new SmtpClient(_emailDto.SmtpServer, _emailDto.SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailDto.SmtpUsername, _emailDto.SmtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailDto.FromEmail),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                return true;

            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"SMTP error occurred: {smtpEx.Message}");
            throw new EmailSendingException("SMTP error occurred while sending email", smtpEx);
        }
        catch (InvalidOperationException invalidOpEx)
        {
            Console.WriteLine($"Invalid operation error occurred: {invalidOpEx.Message}");
            throw new EmailSendingException("Invalid operation occurred while sending email", invalidOpEx);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }



    }
}
