namespace Avalanche.ReadModel.Queries
{
    public class GetUserQuery : IQuery
    {
        public string Username { get; set; }

        public string UserId {  get; set; }
    }
}
