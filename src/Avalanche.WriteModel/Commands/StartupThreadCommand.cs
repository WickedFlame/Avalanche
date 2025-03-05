using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Avalanche.WriteModel.Commands
{
    public class StartupThreadCommand : ICommand
    {
        public StartupThreadCommand()
        {
            Time = DateTime.Now;
        }

        public string TestId { get; set; }

        public DateTime Time { get; set; }

        public string Category { get; set; }

        public string Module { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsWarmup { get; set; }
    }
}
