using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ChatServer.Models;

namespace ChatServer.Services
{
    public class ConnectedUsersService : IConnectedUsersService
    {
        private readonly ConcurrentDictionary<string, User> _users;

        public ConnectedUsersService()
        {
            _users = new ConcurrentDictionary<string, User>();
        }

        public bool AddUser(string connectionId, string userName)
        {
            if (string.IsNullOrEmpty(connectionId) && string.IsNullOrEmpty(userName)) return false;
            try
            {
                return _users.TryAdd(connectionId, new User { UserName = userName });
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error adding new user");
            }
        }

        public Task<bool> AddUserAsync(string connectionId, string userName)
        {
            return Task.Run(() =>
            {
                return AddUser(connectionId, userName);
            });
        }

        public string GetUserNameByConnectionId(string connectionId)
        {
            if (String.IsNullOrEmpty(connectionId)) return "";
            User user;
            if (_users.TryGetValue(connectionId, out user))
            {
                return user?.UserName;
            }
            return "";
        }

        public Task<User> RemoveUserAsync(string connectionId)
        {
            return Task.Run(() =>
            {
                return RemoveUser(connectionId);
            });
        }

        public User RemoveUser(string connectionId)
        {
            if (String.IsNullOrEmpty(connectionId)) return null;
            User user;
            if (_users.TryRemove(connectionId, out user))
                return user;
            return null;
        }
    }
}