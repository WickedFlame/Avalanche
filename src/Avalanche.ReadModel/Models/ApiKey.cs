namespace Avalanche.ReadModel.Models
{
    public class ApiKey
    {
        public string Name {  get; set; }

        public string Value { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }
    }
}
