namespace Avalanche.Runner.Logging
{
    public interface ITestResultCollector
    {
        void Add(LogEvent metric);

        void End();
    }
}