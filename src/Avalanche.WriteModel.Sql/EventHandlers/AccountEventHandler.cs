using Avalanche.DataSource;
using Avalanche.DataSource.DTO;
using Avalanche.WriteModel.Events;
using SqlKata.Execution;

namespace Avalanche.WriteModel.Sql.EventHandlers
{
    public class AccountEventHandler :
        IEventHandler<CreateUserEvent>,
        IEventHandler<DeleteUserEvent>,
        IEventHandler<UpdatePasswordEvent>
    {
        private readonly IProjectionConnectionBuilder _builder;

        public AccountEventHandler(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public void Handle(CreateUserEvent @event)
        {
            var userId = Guid.NewGuid().ToString();

            var db = _builder.Build();
            db.Query(nameof(Users))
                .Insert(new
                {
                    Id = userId,
                    Username = @event.Username,
                    Password = @event.Password,
                    Name = @event.Name
                });

            var roles = db.Query(nameof(Roles))
                .Select()
                .Get<Roles>();

            foreach (var role in @event.Roles)
            {
                var tmp = roles.FirstOrDefault(r => r.Name.ToLower() == role.ToLower());
                if(tmp ==  null)
                {
                    continue;
                }

                db.Query(nameof(UserRoles))
                    .Insert(new
                    {
                        UserId = userId,
                        RoleId = tmp.Id
                    });
            }
        }

        public void Handle(DeleteUserEvent @event)
        {
            var db = _builder.Build();
            db.Query(nameof(UserRoles))
                .Where(
                new
                {
                    UserId = @event.UserId,
                })
                .Delete();

            db.Query(nameof(Users))
                .Where(
                new
                {
                    Id = @event.UserId,
                })
                .Delete();
        }

        public void Handle(UpdatePasswordEvent @event)
        {
            var db = _builder.Build();
            db.Query(nameof(Users))
                .Where(
                new
                {
                    Id = @event.UserId,
                })
                .Update(new
                {
                    Password = @event.Password
                });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do stuf here
            }
        }
    }
}
