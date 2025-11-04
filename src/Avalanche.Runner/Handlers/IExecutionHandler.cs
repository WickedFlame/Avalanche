namespace Avalanche.Runner.Handlers
{
    /// <summary>
    /// Handler that can get executed during the run
    /// </summary>
    public interface IExecutionHandler
    {
        /// <summary>
        /// The Method to execute
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        void Execute(MeasureMap.ExecutionContext context, Scenario settings);
    }
}
