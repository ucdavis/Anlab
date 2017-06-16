using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using MailKit.Net.Smtp;
using MimeKit;

namespace Anlab.Core.Services {
    public class MailService {
        private readonly ApplicationDbContext _dbContext;

        public MailService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task EnqueueMessageAsync(MailMessage message)
        {
            _dbContext.Add(message);

            await _dbContext.SaveChangesAsync();
        }
        
        public void SendMessage(MailMessage mailMessage) {
            var message = new MimeMessage ();
    		message.From.Add (new MailboxAddress ("Anlab", "anlab@ucdavis.edu"));
			message.To.Add (new MailboxAddress(mailMessage.SendTo));
			message.Subject = mailMessage.Subject;
            message.Body = new TextPart("html") { Text = mailMessage.Body };

            using (var client = new SmtpClient ()) {
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s,c,h,e) => true;

                // TODO: use authenticated STMP
                client.Connect("smtp.ucdavis.edu", 25, false);
				// client.Connect ("smtp.ucdavis.edu", 587, false);

				// Note: since we don't have an OAuth2 token, disable
				// the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove ("XOAUTH2");

				// Note: only needed if the SMTP server requires authentication
				// client.Authenticate ("joey", "password");

				client.Send (message);
				client.Disconnect (true);
			}
        }
    }
}