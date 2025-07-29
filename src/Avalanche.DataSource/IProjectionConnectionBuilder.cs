using SqlKata.Execution;

namespace Avalanche.DataSource
{
    public interface IProjectionConnectionBuilder
    {
        QueryFactory Build();
    }
}
