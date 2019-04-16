using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var db = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<UserIdentity>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                try
                {
                    DataInitializer.Initialize(db, userManager, roleManager);
                    host.Run();
                }
                catch (DataInitializeException ex)
                {
                    logger.LogError(ex, "An error occurred while initializing data.");
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
