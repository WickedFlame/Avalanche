using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;

namespace Avalanche.ReadModel
{
    public interface IAccountQueryHandler
        : IQueryHandler<User, GetUserQuery>,
        IQueryHandler<IEnumerable<Role>, GetUserRolesQuery>,
        IQueryHandler<IEnumerable<User>, GetUsersQuery>
    {
    }
}
