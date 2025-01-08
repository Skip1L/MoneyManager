using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Helpers;

namespace MoneyManagerAPI.UnitTests
{
    [TestFixture]
    public class PhoneNumberHelperTests
    {
        #region Valid Phone Number Test Cases
        [TestCase("+12025550154", true)] // Valid international phone number with a country code and dashes
        [TestCase("+442079460958", true)] // Valid UK phone number with country code and spaces
        [TestCase("+491701234567", true)] // Valid phone number with area code in parentheses
        [TestCase("+2347031234567", true)] // Valid phone number with dashes
        [TestCase("+380501234567", true)] // Valid phone number with spaces
        public void IsPhoneNumberValid_ShouldReturnTrue_ForValidPhoneNumbers(string phoneNumber, bool expectedResult)
        {
            // Act
            var result = PhoneNumberHelper.IsPhoneNumberValid(phoneNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion

        #region Invalid Phone Number Test Cases
        [TestCase("123", false)] // Invalid phone number (too short)
        [TestCase("123-456", false)] // Invalid phone number (too short)
        [TestCase("800-555-555", false)] // Invalid phone number (too short)
        [TestCase("abc-555-5555", false)] // Invalid phone number (contains letters)
        [TestCase("800-555-55X5", false)] // Invalid phone number (contains an invalid character)
        [TestCase("+1-800-555-55555555", false)] // Invalid phone number (too long)
        [TestCase("+123 45 6789", false)] // Invalid phone number (too short for international format)
        [TestCase("123.45.6789", false)] // Invalid phone number (too short and improper formatting)
        [TestCase("(800) 555-555", false)] // Invalid phone number (too short)
        [TestCase("1234-567-8901", false)] // Invalid phone number (incorrect formatting)
        public void IsPhoneNumberValid_ShouldReturnFalse_ForInvalidPhoneNumbers(string phoneNumber, bool expectedResult)
        {
            // Act
            var result = PhoneNumberHelper.IsPhoneNumberValid(phoneNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion

    }
}
