using Avalanche.DataSource;
using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using SqlKata.Execution;

namespace Avalanche.ReadModel.Sql.QueryHandlers
{
    public class AccountQueryHandler : IAccountQueryHandler
    {
        private readonly IProjectionConnectionBuilder _builder;

        public AccountQueryHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public User Get(GetUserQuery query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.Users))
                .Select()
                .Where(new
                {
                    Username = query.Username,
                })
                .Get<User>()
                .FirstOrDefault();
        }

        public IEnumerable<Role> Get(GetUserRolesQuery query)
        {
            var db = _builder.Build();
            return db.Query(nameof(DataSource.DTO.Roles))
                .Select()
                .Join(nameof(DataSource.DTO.UserRoles), "Roles.Id", "UserRoles.RoleId")
                .Where(new
                {
                    UserId = query.UserId,
                })
                .Get<Role>();

        }
    }
}
