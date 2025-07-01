namespace Avalanche.WriteModel.Sql.IntegrationTests
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

            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateEventStore();
            Avalanche.DataSource.Sqlite.EventStoreBuilder.CreateWriteModel();
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
