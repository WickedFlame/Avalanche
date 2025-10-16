namespace Avalanche.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AppSettingsApiKeyValidator : IApiKeyValidator
    {
        private readonly ISet<string> _keys;


        /// <summary>
        /// 
        /// </summary>
        public AppSettingsApiKeyValidator()
        {
            _keys = new HashSet<string> { "test" };
        }


        /// <summary>
        /// Validate the ApiKey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
            => Task.FromResult(!string.IsNullOrWhiteSpace(apiKey) && _keys.Contains(apiKey));
    }
}
