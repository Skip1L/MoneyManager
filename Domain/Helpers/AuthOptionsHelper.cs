namespace Domain.Helpers
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

        public static int GetTokenExpirationTime()
        {
            var stringValue = Environment.GetEnvironmentVariable("TOKEN_EXPIRATION_TIME") ?? "7200";

            if (int.TryParse(stringValue, out var myNumber))
            {
                return myNumber;
            }

            return 7200;
        }
    }
}
