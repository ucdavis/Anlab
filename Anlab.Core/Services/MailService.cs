using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using Microsoft.Extensions.Options;
using Serilog;
using MailMessage = Anlab.Core.Domain.MailMessage;

namespace Anlab.Core.Services
{
    public interface IMailService
    {
        void EnqueueMessage(MailMessage message);
        void SendMessage(MailMessage mailMessage);
    }

    public class MailService : IMailService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailSettings _emailSettings;

        public MailService(ApplicationDbContext dbContext, IOptions<EmailSettings> emailSettings)
        {
            _dbContext = dbContext;
            _emailSettings = emailSettings.Value;
        }

        public void EnqueueMessage(MailMessage message)
        {
            _dbContext.Add(message);
        }

        public void SendMessage(MailMessage mailMessage)
        {
            Log.Information($"Email Host: {_emailSettings.Host}");

            var message = new System.Net.Mail.MailMessage {From = new MailAddress("anlab@ucdavis.edu", "Anlab")};
            if (mailMessage.SendTo != "anlab-test@ucdavis.edu") //TODO: Remove when we want to start actually emailing people.            
            {
                throw new Exception("The testing email was not used.");
            }

            var sendToEmails = mailMessage.SendTo.Split(';');
            foreach (var sendToEmail in sendToEmails)
            {
                message.To.Add(sendToEmail);
            }

            var bcc = _emailSettings.BccAnlab;
            if (!string.IsNullOrWhiteSpace(bcc))
            {
                message.Bcc.Add(bcc);
            }

            message.Subject = mailMessage.Subject;
            message.IsBodyHtml = false;
            message.Body = mailMessage.Body;
            var mimeType = new System.Net.Mime.ContentType("text/html");
            var alternate = AlternateView.CreateAlternateViewFromString(mailMessage.Body, mimeType);
            message.AlternateViews.Add(alternate);

            using (var client = new SmtpClient(_emailSettings.Host))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                client.Port = _emailSettings.Port;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;

                client.Send(message);
            }

        }
    }
}
