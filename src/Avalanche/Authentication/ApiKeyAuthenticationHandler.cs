using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Avalanche.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ApiKeyAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "ApiKey";
        private const string HeaderName = "X-API-KEY";
        private readonly IApiKeyValidator _validator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="validator"></param>
        public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IApiKeyValidator validator)
            : base(options, logger, encoder)
        {
            _validator = validator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var values))
            {
                // lets Authorization produce 401
                return AuthenticateResult.NoResult(); 
            }

            var apiKey = values.ToString();
            if (!await _validator.IsValidAsync(apiKey, Context.RequestAborted))
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            // Build an authenticated principal (optional: include owner/claims)
            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);
            return AuthenticateResult.Success(ticket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers["WWW-Authenticate"] = $"{SchemeName} realm=\"api\"";
            return Response.WriteAsJsonAsync(new { message = "Unauthorized: Provide X-API-KEY" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return Response.WriteAsJsonAsync(new { message = "Forbidden" });
        }
    }
}
