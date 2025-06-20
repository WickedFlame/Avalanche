using Avalanche.WriteModel.Sqlite;
using System.Data.SQLite;

namespace Avalanche
{
    public static class EventStoreExtensions
    {
        public static void UseSqliteEventStore(this IApplicationBuilder app)
        {
            Avalanche.WriteModel.Sqlite.EventStoreBuilder.CreateEventStore();
        }

        public static void UseSqliteReadModel(this IApplicationBuilder app)
        {
            Avalanche.WriteModel.Sqlite.EventStoreBuilder.CreateWriteModel();
        }
    }
}
