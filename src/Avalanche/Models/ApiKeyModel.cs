using Avalanche.ReadModel.Models;

namespace Avalanche.Models
{
    public class ApiKeyModel
    {
        public ApiKey NewKey { get; set; }

        public IEnumerable<ApiKey> ApiKeys { get; set; }
    }
}
