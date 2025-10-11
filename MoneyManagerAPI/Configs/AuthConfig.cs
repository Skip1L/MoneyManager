namespace MoneyManagerAPI.Configs
{
    public class AuthConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int TokenExpirationTime { get; set; }
        public string NotificationApiKey { get; set; }
    }
}
