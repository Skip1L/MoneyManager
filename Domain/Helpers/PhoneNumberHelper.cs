using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public static class PhoneNumberHelper
    {
        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            string pattern = @"(?:([+]\d{1,4})[-.\s]?)?(?:[(](\d{1,3})[)][-.\s]?)?(\d{1,4})[-.\s]?(\d{1,4})[-.\s]?(\d{1,9})";
            Regex regex = new(pattern, RegexOptions.Compiled);
            return regex.IsMatch(phoneNumber);
        }
    }
}
