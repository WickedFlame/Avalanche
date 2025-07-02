namespace Avalanche.DataSource.Pgsql
{
    public static class Constants
    {
        //private static readonly string _format = "Host=localhost:5432;Database={0};Username=avalancheuser;Password=pGadm1n";
        private static readonly string _format = "Server=host.docker.internal;Port=5432;Database={0};User Id=avalancheuser;Password=pGadm1n";

        public static readonly string ReadModelDatabase = string.Format(_format, "readmodel");

        public static readonly string EventStoreDatabase = string.Format(_format, "eventstore");
    }
}
