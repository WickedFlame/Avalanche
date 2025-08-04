using Avalanche.DataSource;

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
        }
    }
}
