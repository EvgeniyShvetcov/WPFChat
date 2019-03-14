using System;
using ChatServer.Models;
using ChatServer.Services;
using Xunit;

namespace ChatServer.Tests
{
    public class ConnectedUsersServiceTests
    {
        public ConnectedUsersService Service { get; }
        public ConnectedUsersServiceTests()
        {
            Service = new ConnectedUsersService();
        }

        [Fact]
        public void AddUserWithUniqueCID_AddingUserToDictionary()
        {
            var user = new User { UserName = "John" };
            var cid = "CID1";

            var result = Service.AddUser(cid, user.UserName);

            Assert.True(result);
        }

        [Fact]
        public void AddUserWithExistedCID_NotAddingUserToDictionary()
        {
            var user1 = new User { UserName = "John" };
            var user2 = new User { UserName = "Petr" };
            var cid = "CID1";

            Service.AddUser(cid, user1.UserName);
            var result = Service.AddUser(cid, user2.UserName);

            Assert.False(result);
        }

        [Fact]
        public void RemoveUserByExistingCID_RemoveUserFromDictionary_ReturnDeletedUser()
        {
            var user = new User { UserName = "John" };
            var cid = "CID1";

            Service.AddUser(cid, user.UserName);

            var deletedUser = Service.RemoveUser(cid);

            Assert.Same(user.UserName, deletedUser.UserName);
        }

        [Fact]
        public void RemoveUserByNotExistingCID_ReturnNull()
        {
            var cid = "CID1";

            var deletedUser = Service.RemoveUser(cid);

            Assert.Null(deletedUser);
        }
    }
}