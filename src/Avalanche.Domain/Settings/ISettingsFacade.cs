using Avalanche.ReadModel.Models;

namespace Avalanche.Domain.Settings
{
    public interface ISettingsFacade
    {
        void RecreateDatabase();

        IEnumerable<ApiKey> GetApiKeys();

        ApiKey GetApiKey(string name);

        string AddApiKey(string name, Expiration expiration);

        void DeleteApiKey(string name);
    }
}
