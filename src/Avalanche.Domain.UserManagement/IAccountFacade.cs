using Avalanche.ReadModel.Models;

namespace Avalanche.Domain.UserManagement
{
    public interface IAccountFacade
    {
        User ValidateUser(string username, string password);

        bool CreateUser(string username, string password, string name, IEnumerable<string> roles);

        IEnumerable<User> GetUsers();

        User GetUser(string username);

        bool ChangePassword(string userId, string oldpwd, string newpwd, string confirmpwd);
    }
}
