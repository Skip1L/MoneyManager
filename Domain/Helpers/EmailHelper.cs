using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public static class EmailHelper
    {
        private const string _validEmailPattern= @"^(?:[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*|\""([^\x0d\""\x5c\x80-\xff]|\\[\x00-\x7f])*\"")@(?:(?!-)[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*(?<!-)\.[a-zA-Z]{1,}|localhost|\[(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\])$";
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return Regex.IsMatch(email, _validEmailPattern);
        }
    }
}
