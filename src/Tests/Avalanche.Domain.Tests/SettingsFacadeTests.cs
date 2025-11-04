using Avalanche.DataSource;
using Avalanche.Domain.Settings;
using Avalanche.ReadModel;
using Avalanche.WriteModel.Events;
using AwesomeAssertions;
using Broadcast;
using Moq;

namespace Avalanche.Domain.Tests
{
    public class SettingsFacadeTests
    {
        private Mock<ISettingsQueryHandler> _settingsQueryHandler;
        private Mock<IEventBus> _eventBus;
        private Mock<IDataStoreBuilder> _dsb;
        private ISettingsFacade _facade;

        [SetUp]
        public void Setup()
        {
            _settingsQueryHandler = new Mock<ISettingsQueryHandler>();
            _eventBus = new Mock<IEventBus>();
            _dsb = new Mock<IDataStoreBuilder>();

            _facade = new SettingsFacade(_settingsQueryHandler.Object, _eventBus.Object, _dsb.Object);
        }

        [Test]
        public void SettingsFacade_AddApiKey()
        {
            var key = _facade.AddApiKey("testkey", Expiration.Never);
            key.Should().StartWith("Avlch-").And.HaveLength(32);
        }

        [TestCase(Expiration.OneMonth, 1)]
        [TestCase(Expiration.ThreeMonths, 3)]
        [TestCase(Expiration.SixMonths, 6)]
        [TestCase(Expiration.TwelveMonths, 12)]
        public void SettingsFacade_AddApiKey_Expiration(Expiration expiration, int months)
        {
            _facade.AddApiKey("testkey", expiration);
            _eventBus.Verify(x => x.Send(It.Is<AddApiKeyEvent>(e => e.Expires.Month == DateTime.Now.AddMonths(months).Month)));
        }

        [Test]
        public void SettingsFacade_AddApiKey_Expiration_Never()
        {
            _facade.AddApiKey("testkey", Expiration.Never);
            _eventBus.Verify(x => x.Send(It.Is<AddApiKeyEvent>(e => e.Expires == DateTime.MinValue)));
        }

        [Test]
        public void SettingsFacade_DeleteApiKey()
        {
            _facade.DeleteApiKey("testkey");
            _eventBus.Verify(x => x.Send(It.Is<DeleteApiKeyEvent>(e => e.Name == "testkey")));
        }
    }
}