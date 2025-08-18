namespace Avalanche.ReadModel.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
