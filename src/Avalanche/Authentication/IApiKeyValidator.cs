namespace Avalanche.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public interface IApiKeyValidator
    {
        /// <summary>
        /// Validate the apiKey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default);
    }
}
