using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Issues.Models;

namespace Issues.Services
{
    public class UserService : IUserService
    {
        private IList<User> _users;

        public UserService()
        {
            _users = new List<User>();
            _users.Add(new User(1, "fakeuser1"));
            _users.Add(new User(2, "fakeuser2"));
            _users.Add(new User(3, "fakeuser3"));
            _users.Add(new User(4, "fakeuser4"));
        }

        public User GetUserById(int id)
        {
            return GetUserByIdAsync(id).Result;
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return Task.FromResult(_users.Single(u => Equals(u.Id, id)));
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return Task.FromResult(_users.AsEnumerable());
        }
    }

    public interface IUserService
    {
        User GetUserById(int id);
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersAsync();
    }
}
