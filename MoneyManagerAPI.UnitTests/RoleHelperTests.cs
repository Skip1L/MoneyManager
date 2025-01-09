using Domain.Helpers;

namespace MoneyManagerAPI.UnitTests
{
    [TestFixture]
    public class RoleHelperTests
    {
        #region Valid Roles Test Cases
        [TestCase(new string[] { "ADMINISTRATOR" }, true)] // Valid single role that exists in Roles
        [TestCase(new string[] { "DEFAULT_USER" }, true)] // Valid single role that exists in Roles
        [TestCase(new string[] { "DEFAULT_USER", "ADMINISTRATOR" }, true)] // Valid roles with multiple valid entries
        public void IsRolesValid_ShouldReturnTrue_ForValidRoles(string[] roles, bool expectedResult)
        {
            // Act
            var result = RolesHelper.IsRolesValid([.. roles]);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion

        #region Invalid Roles Test Cases
        [TestCase(new string[] { "Admin", "InvalidRole" }, false)] // Invalid role "InvalidRole"
        [TestCase(new string[] { "SuperAdmin" }, false)] // Invalid role "SuperAdmin" not in Roles
        [TestCase(new string[] { "User", "Manager", "NonExistentRole" }, false)] // Invalid role "NonExistentRole"
        [TestCase(new string[] { "Admin", "" }, false)] // Empty string in roles list
        [TestCase(new string[] { "" }, false)] // Single empty string in roles list
        [TestCase(new string[] { "Admin", null }, false)] // Null value in roles list
        [TestCase(new string[] { }, false)] // Empty roles list
        [TestCase(new string[] { "USER" }, false)] // Valid roles with all existing roles
        public void IsRolesValid_ShouldReturnFalse_ForInvalidRoles(string[] roles, bool expectedResult)
        {
            // Act
            var result = RolesHelper.IsRolesValid([.. roles]);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        #endregion
    }
}
