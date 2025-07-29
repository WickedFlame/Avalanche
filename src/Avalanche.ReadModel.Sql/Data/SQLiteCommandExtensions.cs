using Avalanche.ReadModel.Data;
using System.Data;

namespace Avalanche.ReadModel
{
    internal static class SQLiteCommandExtensions
    {
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
