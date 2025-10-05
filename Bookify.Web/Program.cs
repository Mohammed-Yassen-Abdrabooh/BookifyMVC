using Bookify.Web.Core.Mapping;
using Bookify.Web.Seeds;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;

namespace Bookify.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // in case of using ApplicationUser class only without roles
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // in case of using ApplicationUser class with roles Must add DefaultUI and DefaultTokenProviders and The "DefaultTokenProviders" used for reset password and email confirmation
            // Run only For the first time to create the roles in the database Then you can comment it and uncomment the above code
            ////builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            ////       .AddEntityFrameworkStores<ApplicationDbContext>()
            ////       .AddDefaultUI()
            ////       .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            builder.Services.AddExpressiveAnnotations();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Run only for the first time to create the roles in the database Then you can comment it
            //var scopeFactort = app.Services.GetRequiredService<IServiceScopeFactory>();
            //using var scope = scopeFactort.CreateScope();

            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //await DefaultRoles.SeedRolesAsync(roleManager);
            //await DefaultUsers.SeedAdminUserAsync(userManager);


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}