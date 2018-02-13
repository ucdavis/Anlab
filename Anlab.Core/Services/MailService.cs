using System;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

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

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Anlab", "anlab@ucdavis.edu"));
            if (mailMessage.SendTo != "anlab-test@ucdavis.edu") //TODO: Remove when we want to start actually emailing people.            
            {
                throw new Exception("The testing email was not used.");
            }

            message.To.Add(new MailboxAddress(mailMessage.SendTo));
            message.Subject = mailMessage.Subject;
            message.Body = new TextPart("html") {Text = mailMessage.Body};

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // TODO: use authenticated STMP
                // client.Connect("smtp.mailtrap.io", 2525);
                client.Connect (_emailSettings.Host, _emailSettings.Port, false); //If useSsl is true, I get "The handshake failed due to an unexpected packet format"

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_emailSettings.UserName, _emailSettings.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
