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
        [TestCase("+380501234567", true)]
        [TestCase("380501234567", true)]
        [TestCase("0501234567", true)]
        public void IsPhoneNumberValid_ShouldReturnTrue_ForValidPhoneNumbers(string phoneNumber, bool expectedResult)
        {
            // Act
            var result = PhoneNumberHelper.IsPhoneNumberValid(phoneNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion

        #region Invalid Phone Number Test Cases
        [TestCase("380 50 123 45 67", false)]
        [TestCase("(050)123-45-67", false)]
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
