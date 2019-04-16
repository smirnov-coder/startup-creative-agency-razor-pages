using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;

namespace StartupCreativeAgency.Tests.Shared
{
    public class TestHelper
    {
        public static DbContextOptions<ApplicationDbContext> CreateTestDbContextOptions(SqliteConnection connection)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddEntityFrameworkProxies()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseLazyLoadingProxies()
                .UseSqlite(connection)
                .EnableSensitiveDataLogging()
                .UseInternalServiceProvider(serviceProvider);

            return optionsBuilder.Options;
        }

        public static ServiceProvider CreateServiceProvider(SqliteConnection connection)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .AddEntityFrameworkProxies()
                .BuildServiceProvider();

            var services = new ServiceCollection().AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies()
                    .UseSqlite(connection)
                    .UseInternalServiceProvider(serviceProvider);
            });

            services.AddIdentity<UserIdentity, IdentityRole>(options => {
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            return services.BuildServiceProvider();
        }

        public static void SeedDatabaseWithTestData<T>(ApplicationDbContext db, IEnumerable<T> data) where T : class
        {
            db.Set<T>().AddRange(data);
            db.SaveChanges();
        }

        public static async Task PopulateTestUsersAsync(UserManager<UserIdentity> userManager, IEnumerable<DomainUser> users)
        {
            foreach (var user in users)
            {
                await userManager.CreateAsync(user.Identity as UserIdentity, "User123");
            }
        }
    }
}
