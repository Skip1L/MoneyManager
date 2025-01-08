using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public static class PhoneNumberHelper
    {
        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            string pattern = @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,7}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
