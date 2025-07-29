using Microsoft.Extensions.Configuration;

namespace Avalanche.DataSource.Pgsql
{
    public class ConnectionBuilder
    {
        private readonly DatabaseSettings _settings;

        public ConnectionBuilder(string dbName, IConfiguration config)
        {
            _settings = new DatabaseSettings
            {
                URL = config["AV_DB_SERVER"],
                Port = config["AV_DB_PORT"],
                UserName = config["AV_DB_USERNAME"],
                Password = config["AV_DB_PASSWORD"],
                Database = dbName
            };
        }

        public string BuildConnectionString()
        {
            if (string.IsNullOrEmpty(_settings.Port))
            {
                return $"Server={_settings.URL};Database={_settings.Database};User Id={_settings.UserName};Password={_settings.Password}";
            }

            return $"Server={_settings.URL};Port={_settings.Port};Database={_settings.Database};User Id={_settings.UserName};Password={_settings.Password}";
        }
    }
}
