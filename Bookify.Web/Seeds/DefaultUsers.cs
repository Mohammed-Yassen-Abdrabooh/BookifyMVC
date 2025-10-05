using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = new()
            {
                UserName = "admin", // change "admin" to Email in Database To login with email but We Solve The problem in the Login Page because in Identity By default use Email and UserName are the same To Login
                Email = "admin@bookify.com",
                FullName = "Administrator",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByNameAsync(admin.UserName);
            if (user is null)
            {
                await userManager.CreateAsync(admin, "P@ssord123");
                await userManager.AddToRoleAsync(admin,AppRoles.Admin);
            }
        }
    }
}
