using Domain.Helpers;

namespace MoneyManagerAPI.UnitTests
{
    [TestFixture]
    public class EmailHelperTests
    {
        #region Valid Email Test Cases
        [TestCase("test@example.com", true)] // Simple valid email
        [TestCase("user.name@example.com", true)] // Valid with dot in local part
        [TestCase("user@domain.com", true)] // Basic valid email
        [TestCase("user@sub.domain.com", true)] // Valid email with subdomain
        [TestCase("user@[192.168.1.1]", true)] // Valid email with IP address as domain
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
        [TestCase("user@domain", false)] // Missing TLD
        [TestCase("user@domain@domain.com", false)] // Multiple '@'
        [TestCase("user@.domain.com", false)] // Leading dot in domain
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