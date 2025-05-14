using System.Net;

namespace Avalanche.WriteModel.Commands
{
    public class IterationFailedCommand : ICommand
    {
        public DateTime Time { get; set; }

        public string TestId { get; set; }

        public string TestName { get; set; }

        public int Thread { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
