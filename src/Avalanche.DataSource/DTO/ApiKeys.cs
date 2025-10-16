using System;

namespace Avalanche.DataSource.DTO
{
    public class ApiKeys
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }
    }
}
