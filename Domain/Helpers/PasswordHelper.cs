using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public static class PasswordHelper
    {   
        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            string pattern = @"^(?=.*?[A-Z])(?=(.*[a-z]))(?=(.*[\d]))(?=(.*[^a-zA-Z0-9])).{8,}$";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return regex.IsMatch(password);
        }
    }
}
