using Microsoft.Extensions.Configuration;
using Moq;

namespace Avalanche.ReadModel.Sql.IntegrationTests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            if(!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            if (File.Exists("data/readmodel.db"))
            {
                File.Delete("data/readmodel.db");
            }

            if (File.Exists("data/eventstore.db"))
            {
                File.Delete("data/eventstore.db");
            }

            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateEventStore(Mock.Of<IConfiguration>());
            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateWriteModel(Mock.Of<IConfiguration>());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            //if (File.Exists("data/readmodel.db"))
            //{
            //    File.Delete("data/readmodel.db");
            //}

            //if (File.Exists("data/eventstore.db"))
            //{
            //    File.Delete("data/eventstore.db");
            //}
        }
    }
}
