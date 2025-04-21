using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SocialMedia.Repositories;
using SocialMedia.Models;

namespace SocialMedia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddRazorPages();
            builder.Configuration.AddUserSecrets<Program>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["Database"]);
            });

            builder.Services.AddScoped<IRepository<ReelPostings>, ReelPostingRepositories>();

            

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            builder.Services.AddSingleton<IEmailSender,EmailSender>();
            builder.Services.AddAuthentication().AddCookie();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                // es 2 line uzrunvelyofs Rolebis da Userebis damatebas databasesis tabelshi
                RoleSeeder.SeedRolesAsync(services).Wait();
                UserSeeder.SeedUserAsync(services).Wait();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapStaticAssets();

            app.UseEndpoints(endpoints => {
                endpoints.MapHub<ChatHub>("/chatHub").RequireAuthorization();
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
