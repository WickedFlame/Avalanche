using EnumerateIt;

namespace Avalanche.Domain
{
    public class TestRunnerType : Enumeration
    {
        public TestRunnerType(string name) : base(name)
        {
        }

        public static readonly TestRunnerType External = new(nameof(External));

        public static readonly TestRunnerType Local = new(nameof(Local));
    }
}
