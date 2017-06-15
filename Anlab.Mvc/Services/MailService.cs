using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;

namespace AnlabMvc.Services {
    public class MailService {
        private ApplicationDbContext _dbContext;

        public MailService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task EnqueueMessageAsync(MailMessage message)
        {
            _dbContext.Add(message);

            await _dbContext.SaveChangesAsync();
        }
    }
}