using System.Threading.Tasks;

namespace AnlabMvc.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
