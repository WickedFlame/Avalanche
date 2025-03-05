using Avalanche.WriteModel;

namespace Avalanche.Runner.Logging
{
    //
    // Logger is per Testrun
    // LogCollection is per thread
    //

    public class TestResultsFacory
    {
        private bool _isRunning = true;
        private readonly TestRunData _data;

        public bool IsRunning => _isRunning;

        public TestResultsFacory(TestRunData data)
        {
            _data = data;
        }

        public TestResultsCollection StartNew(string name)
        {
            //
            // is created per thread

            if (!_isRunning)
            {
                return null;
            }

            var collection = new TestResultsCollection(name);
            _data.Collections.Add(collection);

            return collection;
        }

        public void End()
        {
            _isRunning = false;
        }
    }

}
