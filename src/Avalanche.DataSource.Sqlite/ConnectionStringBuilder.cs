using System;
using System.IO;

namespace Avalanche.DataSource.Sqlite
{
    public class ConnectionStringBuilder
    {
        private readonly string _connectionString;

        public ConnectionStringBuilder(string dbName)
        {
            var dataPath = GetDataPath();
            _connectionString = $"Data Source={dataPath}/{dbName}.db";
        }

        public string BuildConnectionString()
        {
            return _connectionString;
        }

        public static string GetDataPath()
        {
            var path = Environment.GetEnvironmentVariable("DATA_PATH");
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                return path;
            }

            path = "../data";
            if (Directory.Exists(path))
            {
                return path;
            }

            path = "./data";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
