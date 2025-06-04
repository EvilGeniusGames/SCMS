using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SCMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCMS.Classes
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string adminEmail = configuration["AdminEmail"] ?? Environment.GetEnvironmentVariable("AdminEmail") ?? "admin@example.com";
            string adminPassword = configuration["AdminPassword"] ?? Environment.GetEnvironmentVariable("AdminPassword") ?? "P@ssword1";
            string adminUsername = configuration["AdminUsername"] ?? Environment.GetEnvironmentVariable("AdminUsername") ?? "AdminUser";

            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            bool isDefaultPassword = adminPassword == "P@ssword1";
            bool isDefaultEmail = adminEmail == "admin@example.com";
            bool isDefaultUsername = adminUsername == "Admin";

            bool mustChangePassword = isDefaultPassword || isDefaultEmail || isDefaultUsername;

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    MustChangePassword = mustChangePassword
                };

                // Validate user and password
                var passwordValidator = serviceProvider.GetRequiredService<IPasswordValidator<ApplicationUser>>();
                var passwordResult = await passwordValidator.ValidateAsync(userManager, user, adminPassword);

                var userValidators = serviceProvider.GetServices<IUserValidator<ApplicationUser>>();
                var userErrors = new List<IdentityError>();

                foreach (var validator in userValidators)
                {
                    var vresult = await validator.ValidateAsync(userManager, user);
                    if (!vresult.Succeeded)
                    {
                        userErrors.AddRange(vresult.Errors);
                    }
                }

                if (!passwordResult.Succeeded || userErrors.Any())
                {
                    var allErrors = passwordResult.Errors.Concat(userErrors);
                    var message = string.Join(", ", allErrors.Select(e => e.Description));
                    throw new Exception($"[Seeder] Admin user validation failed: {message}");
                }

                try
                {
                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Administrator");
                    }
                    else
                    {
                        var message = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"[Seeder] Admin user creation failed: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Seeder] Exception during CreateAsync:");
                    Console.WriteLine(ex.ToString());
                    throw;
                }

            }
        }
    }
}
