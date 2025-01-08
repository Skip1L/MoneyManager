using Domain.Helpers;

namespace MoneyManagerAPI.UnitTests
{
    [TestFixture]
    public class EmailHelperTests
    {
        #region Valid Email Test Cases
        [TestCase("test@example.com", true)] // Simple valid email
        [TestCase("user.name+tag+sorting@example.com", true)] // Valid with tags
        [TestCase("x@example.com", true)] // Valid single-character local part
        [TestCase("example-indeed@strange-example.com", true)] // Valid with hyphen in domain
        [TestCase("user@[192.168.1.1]", true)] // Valid with IP as domain
        [TestCase("\"very.unusual.@.unusual.com\"@example.com", true)] // Quoted local part
        [TestCase("example@localhost", true)] // Valid local domain
        [TestCase("example@s.solutions", true)] // Valid single-letter TLD
        [TestCase("a@b.c", true)] // Minimal valid email
        [TestCase("example@sub.example.com", true)] // Valid subdomain
        public void IsEmailValid_ShouldReturnTrue_ForValidEmails(string email, bool expectedResult)
        {
            // Act
            var result = EmailHelper.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion

        #region Invalid Email Test Cases
        [TestCase("", false)] // Empty string
        [TestCase(null, false)] // Null string
        [TestCase("plainaddress", false)] // Missing '@'
        [TestCase("@missinglocal.com", false)] // Missing local part
        [TestCase("missingdomain@", false)] // Missing domain
        [TestCase("missing@domain..com", false)] // Consecutive dots in domain
        [TestCase("user@@example.com", false)] // Double '@'
        [TestCase(".user@example.com", false)] // Leading dot in local part
        [TestCase("user.@example.com", false)] // Trailing dot in local part
        [TestCase("user..user@example.com", false)] // Consecutive dots in local part
        [TestCase("user@.example.com", false)] // Leading dot in domain
        [TestCase("user@com", false)] // Missing TLD
        [TestCase("user@domain,com", false)] // Invalid character in domain
        [TestCase("user@domain@domain.com", false)] // Multiple '@'
        [TestCase("user@domain-.com", false)] // Domain ends with a hyphen
        [TestCase("user@-domain.com", false)] // Domain starts with a hyphen
        [TestCase("user@domain..com", false)] // Consecutive dots in domain
        public void IsEmailValid_ShouldReturnFalse_ForInvalidEmails(string email, bool expectedResult)
        {
            // Act
            var result = EmailHelper.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion
    }
}