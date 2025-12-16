using Bookify.Web.Core.Mapping;
using Bookify.Web.Data;
using Bookify.Web.Helpers;
using Bookify.Web.Seeds;
using Bookify.Web.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using WhatsAppCloudApi.Extensions;


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

            #region Why Do Not Use Identity Default
            // in case of using ApplicationUser class only without roles
            // and This Not Use Roles in the project if you add Admin Account with Admin Role and Assign To Controller Not Accessed
            // For only Admins and You Use This DefautIdentity The Project Will Not Work With Roles and Give you Access Denied
            ////builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            ////    .AddEntityFrameworkStores<ApplicationDbContext>();

            #endregion

            #region Use Identity With Roles if You Assigned Roles in Project 
            // in case of using ApplicationUser class with roles Must add DefaultUI and DefaultTokenProviders and The "DefaultTokenProviders" used for reset password and email confirmation
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                   .AddEntityFrameworkStores<ApplicationDbContext>()
                   .AddDefaultUI()
                   .AddDefaultTokenProviders();
            #endregion

            // Add Services For Image Service
            builder.Services.AddTransient<IImageService, ImageService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();
            // Identity Password settings. There are The Default Settings Worked Without Write it But i'm Write it To Change Some Settings 
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings. I Will Do The Same Settings But using Regex in UserFormViewModel at ClientSideValidation
                //options.Password.RequireDigit = true;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;//Minumum Length of Password
                //options.Password.RequiredUniqueChars = 1;

                // Configure User settings
                options.User.RequireUniqueEmail = true;

                // Default Lockout settings. You Can Change it If You Want
                // Default TimeSpoan is 5 minutes and MaxFailedAccessAttempts is 5
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 3;
                //options.Lockout.AllowedForNewUsers = true;
            });
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
            // This Service Used To :If an User Account Was Deleted or Deleted by its Role => Cannot Do Any Action if he Was Loighined in the App
            // it Will Take Out To Login Page Again To Take The New Claims Or Not Access To Login Application
            builder.Services.Configure<SecurityStampValidatorOptions>(options => 
                options.ValidationInterval = TimeSpan.Zero);
            builder.Services.AddControllersWithViews();
            // Configure Data Protection To Sequered Application Like Id 
            builder.Services.AddDataProtection().SetApplicationName(nameof(Bookify));

            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
            builder.Services.AddExpressiveAnnotations();

            // Configure WhatsApp API Client Service This Package Created By Elhelaly
            builder.Services.AddWhatsAppApiClient(builder.Configuration);

            // Add HangFire Package Services Configuration To do Background Jobs
            builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            builder.Services.AddHangfireServer();

            // Configure Authorization Policy for Hangfire Dashboard but not only For Hangfire You Can Use it in any Controller or Action
            // Yo Can Add it to anything in App not only used for Hangfire Dashboard , its only Example Apply this policy
            builder.Services.Configure<AuthorizationOptions>(options => options.AddPolicy("AdminsOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            }
            ));

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

            var scopeFactort = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactort.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await DefaultRoles.SeedRolesAsync(roleManager);
            await DefaultUsers.SeedAdminUserAsync(userManager);

            // Configure Hangfire Dashboard Middleware and Recurring Jobs To Prepare Expiration Alert This Method "PrepareExpirationAlert()" Exists in Tasks Folder at HangFireTasks.cs
            app.UseHangfireDashboard("/hangfire", new DashboardOptions 
            { 
                DashboardTitle = "Bookify Dashboard",
                IsReadOnlyFunc = (DashboardContext context) => true, // Make the Dashboard Read-Only
                Authorization = new IDashboardAuthorizationFilter[]
                {
                    new HandfireAuthorizationFilter("AdminsOnly") // Only Admins Can Access Hangfire Dashboard
                }
            });
            // To Run Recurring Job At The Application Start
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var whatsAppClient = scope.ServiceProvider.GetRequiredService<IWhatsAppClient>();
            var emailBodyBuilder = scope.ServiceProvider.GetRequiredService<IEmailBodyBuilder>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            // The object of HangFireTasks to access the PrepareExpirationAlert method need All the dependencies to be injected
            var hangFireTasks = new HangFireTasks(dbContext, webHostEnvironment, whatsAppClient, emailBodyBuilder, emailSender);

            RecurringJob.AddOrUpdate("PrepareExpirationAlertJob", () => hangFireTasks.PrepareExpirationAlert(), "0 14 * * *"); // Daily at 2:00 PM =>From This Site To Learn Cron Expression => crontab.cronhub.io/

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}