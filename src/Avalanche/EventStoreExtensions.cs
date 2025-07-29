using Avalanche.WriteModel.Sql;
using System.Data.SQLite;

namespace Avalanche
{
    public static class EventStoreExtensions
    {
        public static void UseSqliteEventStore(this IApplicationBuilder app, IConfiguration config)
        {
            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateEventStore(config);
        }

        public static void UseSqliteReadModel(this IApplicationBuilder app, IConfiguration config)
        {
            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateWriteModel(config);
        }




        public static void UsePostgresEventStore(this IApplicationBuilder app, IConfiguration config)
        {
            Avalanche.DataSource.Pgsql.EventStoreBuilder.CreateEventStore(config);
        }

        public static void UsePostgresReadModel(this IApplicationBuilder app, IConfiguration config)
        {
            Avalanche.DataSource.Pgsql.EventStoreBuilder.CreateWriteModel(config);
        }
    }
}
