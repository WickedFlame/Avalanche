using System.Net;

namespace Avalanche.Runner
{
    public class TestCase : TestCaseConfig
    {
        /// <summary>
        /// The urls that are run in each test
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// The name of the testcase
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The config that is run for initialization
        /// </summary>
        public InitConfig Init { get; set; }

        public Authorization Authorization { get; set; }
    }

    public class InitConfig
    {
        public string Url { get; set; }
    }

    /// <summary>
    /// Authorization node for the configuration
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Supported values:
        /// - oauth
        /// </summary>
        public string Type { get; set; } = "oauth";

        /// <summary>
        /// Supported GrantTypes:
        /// - password
        /// - client_credentials
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// URL of the Authority to authenticate agains
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Username for password grant_type
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for password grant_type
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Scopes for the authentication
        /// </summary>
        public string Scope { get; set; }
    }
}
