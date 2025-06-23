using SqlKata.Execution;

namespace Avalanche.DataSource
{
    public interface IEventStoreConnectionBuilder
    {
        QueryFactory Build();
    }
}
