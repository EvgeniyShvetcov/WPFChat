using System.Threading.Tasks;
using ChatServer.Models;

namespace ChatServer.Services
{
    public interface IConnectedUsersService
    {
        bool AddUser(string connectionId, string userName);
        Task<bool> AddUserAsync(string connectionId, string userName);
        User RemoveUser(string connectionId);
        Task<User> RemoveUserAsync(string connectionId);
        string GetUserNameByConnectionId(string connectionId);

    }
}