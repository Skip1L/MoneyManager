using Microsoft.Extensions.Configuration;

namespace Domain.Helpers
{
    public static class AuthOptionsHelper
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetIssuer() =>
            _configuration?["Auth:Issuer"] ?? "MoneyTrackerIssuer";

        public static string GetAudience() =>
            _configuration?["Auth:Audience"] ?? "MoneyTrackerHost";

        public static string GetSecretKey() =>
            _configuration?["Auth:SecretKey"] ?? "sjgienghs;vcsfrtuifs1)d56fdsaw67";

        public static int GetTokenExpirationTime()
        {
            var stringValue = _configuration?["Auth:TokenExpirationTime"] ?? "7200";
            return int.TryParse(stringValue, out var result) ? result : 7200;
        }
    }
}
