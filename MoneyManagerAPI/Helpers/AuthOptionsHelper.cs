using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MoneyManagerAPI.Helpers
{
    public static class AuthOptionsHelper
    {
        public static string GetIssuer()
        {
            return Environment.GetEnvironmentVariable("ISSUER") ?? "MoneyTrackerIssuer";
        }

        public static string GetAudience()
        {
            return Environment.GetEnvironmentVariable("AUDIENCE") ?? "MoneyTrackerHost";
        }

        public static string GetSecretKey()
        {
            return Environment.GetEnvironmentVariable("API_KEY") ?? "sjgienghs;vcsfrtuifs1)d56fdsaw67";
        }
    }
}
