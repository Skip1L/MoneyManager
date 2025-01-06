using System.Data;
using System.Net;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MoneyManagerAPI.Helpers
{
    public static class EntitySeeder
    {

        public static async Task InitializeDbForIdentity(this IApplicationBuilder app)
        {
            await InitializeDbForRoles(app);

            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "qwerty@gmail.com";
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "qwer1234";

            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            var personManager = serviceScope?.ServiceProvider.GetRequiredService<UserManager<User>>()
                ?? throw new Exception("An error occured while seeding Identity data");

            if (await personManager.FindByEmailAsync(adminEmail) != null)
            {
                return;
            }

            var admin = new User
            {
                UserName = adminEmail,
                EmailConfirmed = true,
                FirstName = Environment.GetEnvironmentVariable("ADMIN_FIRST_NAME") ?? "Illia",
                LastName = Environment.GetEnvironmentVariable("ADMIN_LAST_NAME") ?? "Skipar",
                PhoneNumber = Environment.GetEnvironmentVariable("ADMIN_PHONE_NUMBER") ?? "+380997379641",
                Email = adminEmail,
                CreatedAt = DateTime.UtcNow,
                DateOfBirth = new DateTime(2003, 8, 2),
            };

            await personManager.CreateAsync(admin, adminPassword);
            await personManager.AddToRoleAsync(admin, Roles.ADMINISTRATOR);
        }

        private static async Task InitializeDbForRoles(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            var roleManager = (serviceScope?.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>())
                ?? throw new Exception("An error occured while seeding Roles data");

            var rolesToCreate = typeof(Roles).GetFields()
                .Select(field => field.GetValue(null)?.ToString())
                .Where(roleName => !string.IsNullOrEmpty(roleName))
                .ToList();

            foreach (var roleName in rolesToCreate)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var identityRole = new IdentityRole<Guid>() { Name = roleName };
                    await roleManager.CreateAsync(identityRole);
                }
            }
        }
    }
}
