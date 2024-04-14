using Domain.Interfaces.Mails;
using Domain.Objects.Requests.Machine;
using Domain.Utils.Constants;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infra.CrossCutting.Mails
{
    public class MailerService : IMailerService
    {
        public async Task SendMail(MailDataRequest mailDataRequest, string htmlBody)
        {
            try
            {
                using var emailMessage = new MimeMessage();

                var emailFrom = new MailboxAddress(MailSetting.SenderName, MailSetting.SenderEmail);
                emailMessage.From.Add(emailFrom);
                var emailTo = new MailboxAddress(mailDataRequest.EmailToName, mailDataRequest.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = mailDataRequest.EmailSubject;

                var emailBodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };

                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                using var mailClient = new SmtpClient();
                await mailClient.ConnectAsync(MailSetting.Server, MailSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await mailClient.AuthenticateAsync(MailSetting.UserName, MailSetting.Password);

                await mailClient.SendAsync(emailMessage);

                await mailClient.DisconnectAsync(true);
            }
            catch
            {
                throw new InvalidOperationException("ErrorSendingEmail");
            }
        }
    }
}
