using Microsoft.AspNetCore.Identity;
using SocialMedia.Constants;
namespace SocialMedia.Data
{
    public class UserSeeder
    {
        public static async Task SeedUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await CreateUserWithRole(userManager,"admin", "admin@localhost", "Admin!23", Roles.Admin);
            await CreateUserWithRole(userManager,"defaultUser", "user@localhost", "User!23", Roles.User);
        }
        private static async Task CreateUserWithRole(
            UserManager<IdentityUser> userManager, string username,string email,  string password,
            string role)
        {
            if(await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    UserName = username,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                } else
                {
                    throw new Exception($"Failed to create user with email {user.Email}. Errors: {string.Join(",", result.Errors)}");
                }
            }
        }
    }
}
