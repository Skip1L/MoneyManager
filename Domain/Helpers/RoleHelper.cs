using Domain.Constants;

namespace Domain.Helpers
{
    public static class RolesHelper
    {
        public static bool IsRolesValid(List<string> roles)
        {
            if (roles.Count == 0)
            {
                return false;
            }

            var existedRoles = typeof(Roles)
                .GetFields()
                .Select(field => field.GetValue(null)?.ToString())
                .Where(roleName => !string.IsNullOrEmpty(roleName))
                .ToList();

            return roles.All(existedRoles.Contains);
        }
    }
}
