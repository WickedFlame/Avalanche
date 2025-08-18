using Avalanche.DataSource;
using Avalanche.Domain.UserManagement;

namespace Avalanche
{
    public static class EventStoreExtensions
    {
        public static void UseEventStore(this IApplicationBuilder app)
        {
            var store = app.ApplicationServices.GetService<IDataStoreBuilder>();
            store.CreateEventStore();
        }

        public static void UseReadModel(this IApplicationBuilder app)
        {
            var store = app.ApplicationServices.GetService<IDataStoreBuilder>();
            store.CreateWriteModel();

            var facade = app.ApplicationServices.GetService<IAccountFacade>();
            facade.CreateUser("admin", "admin", "Admin", ["Admin"]);
        }
    }
}
