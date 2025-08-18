using Avalanche.DataSource.DTO;
using Avalanche.ReadModel;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.WriteModel.Events;
using Broadcast;
using Microsoft.AspNetCore.Identity;

namespace Avalanche.Domain.UserManagement
{
    public class AccountFacade : IAccountFacade
    {
        private readonly IAccountQueryHandler _queryHandler;
        private readonly IEventBus _eventBus;

        public AccountFacade(IAccountQueryHandler queryHandler, IEventBus eventBus)
        {
            _queryHandler = queryHandler;
            _eventBus = eventBus;
        }

        public User ValidateUser(string username, string password)
        {
            var user = _queryHandler.Get(new GetUserQuery { Username = username });
            
            if (user == null)
            {
                return null;
            }

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(username, user.Password, password);

            if (result != PasswordVerificationResult.Success)
            {
                return null;
            }

            user.Roles = _queryHandler.Get(new GetUserRolesQuery { UserId = user.Id }).Select(r => r.Name);

            return user;
        }

        public bool CreateUser(string username, string password, string name, IEnumerable<string> roles)
        {
            var user = _queryHandler.Get(new GetUserQuery { Username = username });
            if(user != null)
            {
                return false;
            }

            var hasher = new PasswordHasher<string>();

            _eventBus.Publish(Guid.NewGuid().ToString(), DateTime.Now, new CreateUserEvent
            {
                Username = username,
                Password = hasher.HashPassword(username, password),
                Name = name,
                Roles = roles
            });

            return true;
        }
    }
}
