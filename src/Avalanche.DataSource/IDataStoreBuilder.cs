namespace Avalanche.DataSource
{
    public interface IDataStoreBuilder
    {
        void RecreateWriteModel();

        void CreateEventStore();

        void CreateWriteModel();
    }
}
