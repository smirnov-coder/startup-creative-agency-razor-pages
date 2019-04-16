using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Integration.Services
{
    public class UserServiceTests : IDisposable
    {
        private string _connectionString = "DataSource=:memory:";
        private SqliteConnection _connection;

        public void Dispose()
        {
            _connection?.Dispose();
        }

        [Fact]
        public async Task GetDisplayedTeamMembersAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var services = TestHelper.CreateServiceProvider(_connection))
                {
                    var testUsers = GetTestUserCollection();
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                    }
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userRepository = new UserRepository(genericRepository, userManager);
                        var target = new UserService(userRepository);
                        int expectedTeamMemberCount = 4,
                            expectedSocialLinkCount = 4;

                        var result = await target.GetDisplayedTeamMembersAsync();

                        Assert.Equal(expectedTeamMemberCount, result.Count);
                        Assert.Equal("user1", result.First().Identity.UserName);
                        Assert.Equal("FirstName #1", result.First().Profile.FirstName);
                        Assert.Equal("LastName #1", result.First().Profile.LastName);
                        Assert.Equal("Job #1", result.First().Profile.JobPosition);
                        Assert.Equal("Path #1", result.First().Profile.PhotoFilePath);
                        Assert.True(result.First().Profile.IsReadyForDisplay);
                        Assert.True(result.First().Profile.DisplayAsTeamMember);
                        Assert.Equal(expectedSocialLinkCount, result.First().Profile.SocialLinks.Count);
                        Assert.Contains(result.First().Profile.SocialLinks, x => x.NetworkName == "Facebook");
                        Assert.Contains(result.First().Profile.SocialLinks, x => x.NetworkName == "Twitter");
                        Assert.Contains(result.First().Profile.SocialLinks, x => x.NetworkName == "GooglePlus");
                        Assert.Contains(result.First().Profile.SocialLinks, x => x.NetworkName == "Linkedin");
                        Assert.Equal("user4", result.Last().Identity.UserName);
                        Assert.Equal("FirstName #4", result.Last().Profile.FirstName);
                        Assert.Equal("LastName #4", result.Last().Profile.LastName);
                        Assert.Equal("Job #4", result.Last().Profile.JobPosition);
                        Assert.Equal("Path #4", result.Last().Profile.PhotoFilePath);
                        Assert.True(result.Last().Profile.IsReadyForDisplay);
                        Assert.True(result.Last().Profile.DisplayAsTeamMember);
                        Assert.Equal(expectedSocialLinkCount, result.Last().Profile.SocialLinks.Count);
                        Assert.Contains(result.Last().Profile.SocialLinks, x => x.NetworkName == "Facebook");
                        Assert.Contains(result.Last().Profile.SocialLinks, x => x.NetworkName == "Twitter");
                        Assert.Contains(result.Last().Profile.SocialLinks, x => x.NetworkName == "GooglePlus");
                        Assert.Contains(result.Last().Profile.SocialLinks, x => x.NetworkName == "Linkedin");
                    }
                }
            }
        }

        [Fact]
        public async Task GetUserAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var services = TestHelper.CreateServiceProvider(_connection))
                {
                    var testUsers = GetTestUserCollection();
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                    }
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userRepository = new UserRepository(genericRepository, userManager);
                        var target = new UserService(userRepository);
                        int expectedSocialLinkCount = 4;

                        var result = await target.GetUserAsync("user5");

                        Assert.NotNull(result);
                        Assert.Equal("user5", result.Identity.UserName);
                        Assert.Equal("FirstName #5", result.Profile.FirstName);
                        Assert.Equal("LastName #5", result.Profile.LastName);
                        Assert.Equal("Job #5", result.Profile.JobPosition);
                        Assert.Equal("Path #5", result.Profile.PhotoFilePath);
                        Assert.True(result.Profile.IsReadyForDisplay);
                        Assert.False(result.Profile.DisplayAsTeamMember);
                        Assert.Equal(expectedSocialLinkCount, result.Profile.SocialLinks.Count);
                        Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "Facebook");
                        Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "Twitter");
                        Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "GooglePlus");
                        Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "Linkedin");
                    }
                }
            }
        }

        [Fact]
        public async Task CreateUserAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var scope = TestHelper.CreateServiceProvider(_connection).CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(userManager.Users);
                    Assert.Empty(roleManager.Roles);
                    Assert.True((await roleManager.CreateAsync(new IdentityRole("TestRole"))).Succeeded);
                    var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                    var userRepository = new UserRepository(genericRepository, userManager);
                    var target = new UserService(userRepository);
                    int expectedSocialLinkCount = 4;

                    var result = await target.CreateUserAsync("testuser", "User123", "test@example.com", "TestRole");

                    Assert.NotNull(result);
                    Assert.Equal("testuser", result.Identity.UserName);
                    Assert.True(string.IsNullOrWhiteSpace(result.Profile.FirstName));
                    Assert.True(string.IsNullOrWhiteSpace(result.Profile.LastName));
                    Assert.True(string.IsNullOrWhiteSpace(result.Profile.JobPosition));
                    Assert.True(string.IsNullOrWhiteSpace(result.Profile.PhotoFilePath));
                    Assert.False(result.Profile.IsReadyForDisplay);
                    Assert.False(result.Profile.DisplayAsTeamMember);
                    Assert.Equal(expectedSocialLinkCount, result.Profile.SocialLinks.Count);
                    Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "Facebook");
                    Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "Twitter");
                    Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "GooglePlus");
                    Assert.Contains(result.Profile.SocialLinks, x => x.NetworkName == "Linkedin");
                    Assert.True(result.Profile.SocialLinks.All(x => string.IsNullOrWhiteSpace(x.Url)));
                }
            }
        }

        [Fact]
        public async Task UpdateUserPersonalInfoAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var services = TestHelper.CreateServiceProvider(_connection))
                {
                    var testUsers = GetTestUserCollection();
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                    }
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userRepository = new UserRepository(genericRepository, userManager);
                        var modifiedUser = testUsers.First();
                        var expectedSocialLinkCount = 4;
                        var target = new UserService(userRepository);

                        var result = await target.UpdateUserPersonalInfoAsync(modifiedUser.Identity.UserName,
                            "New FirstName", "New LastName", "New Job", "New Path");

                        Assert.NotNull(result);
                        Assert.Equal(modifiedUser.Identity.UserName, result.Identity.UserName);
                        Assert.Equal("New FirstName", result.Profile.FirstName);
                        Assert.Equal("New LastName", result.Profile.LastName);
                        Assert.Equal("New Job", result.Profile.JobPosition);
                        Assert.Equal("New Path", result.Profile.PhotoFilePath);
                        Assert.True(result.Profile.IsReadyForDisplay);
                        Assert.True(result.Profile.DisplayAsTeamMember);
                        Assert.Equal(expectedSocialLinkCount, result.Profile.SocialLinks.Count);
                        Assert.True(result.LastUpdatedOn >= result.CreatedOn);
                    }
                }
            }
        }

        [Fact]
        public async Task UpdateUserSocialLinksAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var services = TestHelper.CreateServiceProvider(_connection))
                {
                    var testUsers = GetTestUserCollection();
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                    }
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userRepository = new UserRepository(genericRepository, userManager);
                        var modifiedUser = testUsers.First();
                        var expectedSocialLinkCount = 5;
                        var target = new UserService(userRepository);

                        var result = await target.UpdateUserSocialLinksAsync(modifiedUser.Identity.UserName,
                            new SocialLink("Facebook", "New Url"), new SocialLink("Test Name", "Test Url"));

                        Assert.NotNull(result);
                        Assert.Equal(modifiedUser.Identity.UserName, result.Identity.UserName);
                        Assert.Equal("FirstName #1", result.Profile.FirstName);
                        Assert.Equal("LastName #1", result.Profile.LastName);
                        Assert.Equal("Job #1", result.Profile.JobPosition);
                        Assert.Equal("Path #1", result.Profile.PhotoFilePath);
                        Assert.True(result.Profile.IsReadyForDisplay);
                        Assert.True(result.Profile.DisplayAsTeamMember);
                        Assert.Equal(expectedSocialLinkCount, result.Profile.SocialLinks.Count);
                        Assert.Equal("Facebook", result.Profile.SocialLinks.First().NetworkName);
                        Assert.Equal("New Url", result.Profile.SocialLinks.First().Url);
                        Assert.Equal("Test Name", result.Profile.SocialLinks.Last().NetworkName);
                        Assert.Equal("Test Url", result.Profile.SocialLinks.Last().Url);
                        Assert.True(result.LastUpdatedOn >= result.CreatedOn);
                    }
                }
            }
        }

        [Fact]
        public async Task DeleteUserAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var services = TestHelper.CreateServiceProvider(_connection))
                {
                    var testUsers = GetTestUserCollection();
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                    }
                    using (var scope = services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userRepository = new UserRepository(genericRepository, userManager);
                        var userToDelete = testUsers.First();
                        var target = new UserService(userRepository);

                        await target.DeleteUserAsync(userToDelete.Identity.UserName);

                        Assert.Equal(testUsers.Count() - 1, userManager.Users.Count());
                        Assert.Equal(testUsers.Count() - 1, db.DomainUsers.Count());
                        Assert.DoesNotContain(userManager.Users, x => x.UserName == userToDelete.Identity.UserName);
                        Assert.DoesNotContain(db.DomainUsers, x => x.Profile.FirstName == userToDelete.Profile.FirstName);
                    }
                }
            }
        }

        private IEnumerable<DomainUser> GetTestUserCollection()
        {
            var socialLinks = GetTestSocialLinkCollection();
            var users = new DomainUser[]
            {
                new DomainUser(new UserIdentity("user1", "user1@example.com"),
                    new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1", socialLinks)),
                new DomainUser(new UserIdentity("user2", "user2@example.com"),
                    new UserProfile("FirstName #2", "LastName #2", "Job #2", "Path #2", socialLinks)),
                new DomainUser(new UserIdentity("user3", "user3@example.com"),
                    new UserProfile("FirstName #3", "LastName #3", "Job #3", "Path #3", socialLinks)),
                new DomainUser(new UserIdentity("user4", "user4@example.com"),
                    new UserProfile("FirstName #4", "LastName #4", "Job #4", "Path #4", socialLinks)),
                new DomainUser(new UserIdentity("user5", "user5@example.com"),
                    new UserProfile("FirstName #5", "LastName #5", "Job #5", "Path #5", socialLinks))
            };

            foreach (var user in users)
            {
                if (user.Identity.UserName != "user5")
                    user.Profile.ChangeDisplayStatus(true);
            }
            return users;
        }

        private SocialLink[] GetTestSocialLinkCollection()
        {
            return new SocialLink[]
            {
                new SocialLink("Facebook", "Url #1"),
                new SocialLink("Twitter", "Url #2"),
                new SocialLink("GooglePlus", "Url #3"),
                new SocialLink("Linkedin", "Url #4")
            };
        }
    }
}
