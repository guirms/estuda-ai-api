using Domain.Objects.Requests.Machine;

namespace Domain.Interfaces.Mails
{
    public interface IMailerService
    {
        Task SendMail(MailDataRequest mailDataRequest, string htmlBody);
    }
}
