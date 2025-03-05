using Avalanche.ReadModel.Data;
using System.Data;
using System.Data.SQLite;

namespace Avalanche.ReadModel
{
    internal static class SQLiteCommandExtensions
    {
        public static IEnumerable<T> Execute<T>(this SQLiteCommand command) where T : class, new()
        {
            // create data adapter
            var da = new SQLiteDataAdapter(command);
            var table = new DataTable();
            da.Fill(table);

            return Map<T>(table);
        }

        /// <summary>
        /// Map the <see cref="DataTable"/> to a List of <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
		public static IEnumerable<T> Map<T>(DataTable table) where T : class, new()
        {
            var mapper = new EntityMapper<T>();
            var entities = mapper.Map(table);

            return entities;
        }
    }
}
