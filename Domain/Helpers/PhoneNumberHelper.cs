using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public static class PhoneNumberHelper
    {
        private const string _validPhoneNumberPattern = @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,7}$";
        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            return Regex.IsMatch(phoneNumber, _validPhoneNumberPattern);
        }
    }
}
