namespace Avalanche.Models
{
    public class ProfileModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
