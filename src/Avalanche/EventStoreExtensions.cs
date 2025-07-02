using Avalanche.WriteModel.Sql;
using System.Data.SQLite;

namespace Avalanche
{
    public static class EventStoreExtensions
    {
        public static void UseSqliteEventStore(this IApplicationBuilder app)
        {
            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateEventStore();
        }

        public static void UseSqliteReadModel(this IApplicationBuilder app)
        {
            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateWriteModel();
        }




        public static void UsePostgresEventStore(this IApplicationBuilder app)
        {
            Avalanche.DataSource.Pgsql.EventStoreBuilder.CreateEventStore();
        }

        public static void UsePostgresReadModel(this IApplicationBuilder app)
        {
            Avalanche.DataSource.Pgsql.EventStoreBuilder.CreateWriteModel();
        }
    }
}
