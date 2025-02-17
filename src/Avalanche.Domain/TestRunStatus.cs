using EnumerateIt;

namespace Avalanche.Domain
{
    public class TestRunStatus(string name) : Enumeration(name)
    {
        public static readonly TestRunStatus New = new(nameof(New));

        public static readonly TestRunStatus Running = new (nameof(Running));

        public static readonly TestRunStatus Done = new (nameof(Done));
    }
}
