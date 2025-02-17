using MeasureMap.Tracers;

namespace Avalanche.Runner
{
    public class ConsoleResultWriter : IResultWriter
    {
        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
