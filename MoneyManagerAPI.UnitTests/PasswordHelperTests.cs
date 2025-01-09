using Domain.Helpers;

namespace MoneyManagerAPI.UnitTests
{
    [TestFixture]
    public class PasswordHelperTests
    {
        #region Valid Password Test Cases
        [TestCase("Password123!", true)] // Valid password with uppercase, lowercase, number, and special character
        [TestCase("Password1@", true)] // Valid password with uppercase, lowercase, number, and special character
        [TestCase("Pa$$word2025", true)] // Valid password with uppercase, special characters, number, and 8+ characters
        [TestCase("Valid1@Password", true)] // Valid complex password with a mix of required characters
        [TestCase("P@ssw0rd", true)] // Valid password with a mix of uppercase, lowercase, number, and special character
        [TestCase("Strong1@Password", true)] // Valid password with more than 8 characters
        [TestCase("MySecureP@ss1", true)] // Valid complex password
        [TestCase("Ab1@valid", true)] // Valid password with 8 characters, all required components
        [TestCase("a1B@cde!", true)] // Valid password with mixed case and special characters
        [TestCase("ComplexP@ss123", true)] // Valid complex password with length, special characters, and numbers
        public void IsPasswordValid_ShouldReturnTrue_ForValidPasswords(string password, bool expectedResult)
        {
            // Act
            var result = PasswordHelper.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion

        #region Valid Password Test Cases
        [TestCase("password123", false)] // Invalid password with no uppercase or special characters
        [TestCase("password", false)] // Invalid password with no uppercase, number, or special character
        [TestCase("PASSWORD123", false)] // Invalid password with no lowercase or special characters
        [TestCase("Pass123", false)] // Invalid password with no special character and too short
        [TestCase("12345678", false)] // Invalid password with no letters or special characters
        [TestCase("!@#$%^&*", false)] // Invalid password with no letters or numbers
        [TestCase("short1@", false)] // Invalid password with insufficient length
        [TestCase("123456", false)] // Invalid password with no uppercase or special characters
        [TestCase("a@b", false)] // Invalid password with insufficient length and missing components
        [TestCase("P@ss", false)] // Invalid password with too short length
        public void IsPasswordValid_ShouldReturnFalse_ForInvalidPasswords(string password, bool expectedResult)
        {
            // Act
            var result = PasswordHelper.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion
    }
}
