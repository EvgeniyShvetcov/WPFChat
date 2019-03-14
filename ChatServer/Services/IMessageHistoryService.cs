using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Services
{
    public interface IMessageHistoryService
    {
        Task AddMessageToHistoryAsync(string message);
        void AddMessageToHistory(string message);
        Task<IEnumerable<string>> GetMessageHistoryAsync();
        IEnumerable<string> GetMessageHistory();
    }
}