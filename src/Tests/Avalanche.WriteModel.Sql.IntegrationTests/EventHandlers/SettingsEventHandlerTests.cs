using Avalanche.DataSource.DTO;
using Avalanche.DataSource.Sqlite;
using Avalanche.WriteModel.Events;
using Avalanche.WriteModel.Sql.EventHandlers;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework.Internal.Execution;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SQLite;

namespace Avalanche.WriteModel.Sql.IntegrationTests.EventHandlers
{
    public class SettingsEventHandlerTests
    {
        private QueryFactory _db;

        [SetUp]
        public void Setup()
        {
            var connection = new SQLiteConnection($"Data Source=data/{Constants.ReadModel}.db");
            var compiler = new SqliteCompiler();
            _db = new QueryFactory(connection, compiler);
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        [Test]
        public void SettingsEventHandler_AddApiKeyEvent()
        {
            var handler = new SettingsEventHandler(new ProjectionConnectionBuilder(Mock.Of<IConfiguration>()));
            handler.Handle(new AddApiKeyEvent
            {
                Name = "Test",
                Value = "apikey",
                Created = DateTime.Now,
                Expires = DateTime.MinValue
            });

            var apiKey = _db.Query(nameof(ApiKeys))
                .Select()
                .Where("Name", "Test")
                .First<ApiKeys>();

            apiKey.Value.Should().Be("apikey");
            apiKey.Created.Date.Should().Be(DateTime.Now.Date);
            apiKey.Expires.Should().Be(DateTime.MinValue);
        }

        [Test]
        public void SettingsEventHandler_DeleteApiKeyEvent()
        {
            _db.Query(nameof(ApiKeys))
                .Insert(new
                {
                    Name = "TestDelete",
                    Value = "apikey",
                    Created = DateTime.Now
                });

            var handler = new SettingsEventHandler(new ProjectionConnectionBuilder(Mock.Of<IConfiguration>()));
            handler.Handle(new DeleteApiKeyEvent { Name = "TestDelete" });

            _db.Query(nameof(ApiKeys))
                .Select()
                .Where("Name", "TestDelete")
                .Get<ApiKeys>()
                .Should().BeEmpty();
        }
    }
}
