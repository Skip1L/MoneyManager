using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public static class PasswordHelper
    {
        private const string _validPasswordPattern= @"^(?=.*?[A-Z])(?=(.*[a-z]))(?=(.*[\d]))(?=(.*[^a-zA-Z0-9])).{8,}$";

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            return Regex.IsMatch(password, _validPasswordPattern);
        }
    }
}
