using System.Threading.Tasks;

namespace AnlabMvc.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
