using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Infrastructure.Tests.Integration.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private string _connectionString = "DataSource=:memory:";
        private SqliteConnection _connection;

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }

        // EfUserRepository.ListAsync()
        // EfRepository.ListAsync()
        // EfRepository.List(specification)
        [Fact]
        public async Task ListAsync_Good()
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
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var target = new UserRepository(genericRepository, userManager);

                        var result = await target.ListAsync();

                        Assert.Equal(testUsers.Count(), result.Count);
                        Assert.Equal("user1", result.First().Identity.UserName);
                        Assert.Equal("user1@example.com", result.First().Identity.Email);
                        Assert.Equal("FirstName #1", result.First().Profile.FirstName);
                        Assert.Equal("LastName #1", result.First().Profile.LastName);
                        Assert.Equal("Job #1", result.First().Profile.JobPosition);
                        Assert.Equal("Path #1", result.First().Profile.PhotoFilePath);
                        Assert.Equal(4, result.First().Profile.SocialLinks.Count);
                        Assert.Equal("user3", result.Last().Identity.UserName);
                        Assert.Equal("user3@example.com", result.Last().Identity.Email);
                        Assert.Equal("FirstName #3", result.Last().Profile.FirstName);
                        Assert.Equal("LastName #3", result.Last().Profile.LastName);
                        Assert.Equal("Job #3", result.Last().Profile.JobPosition);
                        Assert.Equal("Path #3", result.Last().Profile.PhotoFilePath);
                    }
                }
            }
        }

        // EfUserRepository.GetAsync()
        // EfUserRepository.GetAsync(specification)
        // EfRepository.GetBySpecificationAsync()
        [Fact]
        public async Task GetAsync_Good()
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
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var target = new UserRepository(genericRepository, userManager);
                        string expectedUserName = "user2";

                        var result = await target.GetAsync(expectedUserName);

                        Assert.Equal(expectedUserName, result.Identity.UserName);
                        Assert.Equal("user2@example.com", result.Identity.Email);
                        Assert.Equal("FirstName #2", result.Profile.FirstName);
                        Assert.Equal("LastName #2", result.Profile.LastName);
                        Assert.Equal("Job #2", result.Profile.JobPosition);
                        Assert.Equal("Path #2", result.Profile.PhotoFilePath);
                        Assert.True(result.Profile.IsReadyForDisplay);
                        Assert.True(result.Profile.DisplayAsTeamMember);
                        Assert.Equal(4, result.Profile.SocialLinks.Count);
                        Assert.Equal("Facebook", result.Profile.SocialLinks.First().NetworkName);
                        Assert.Equal("Url #1", result.Profile.SocialLinks.First().Url);
                        Assert.Equal("Linkedin", result.Profile.SocialLinks[3].NetworkName);
                        Assert.Equal("Url #4", result.Profile.SocialLinks[3].Url);
                        Assert.Null(result.CreatedBy);
                        Assert.Null(result.LastUpdatedBy);
                    }
                }
            }
        }

        [Fact]
        public async Task GetAsync_Good_NullReference()
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
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());
                        var target = new UserRepository(genericRepository, userManager);

                        var result = await target.GetAsync("notExisting");

                        Assert.Null(result);
                    }
                }
            }
        }

        // EfUserRepository.CreateAsync()
        // EfRepository.AddAsync()
        [Fact]
        public async Task CreateAsync_Good()
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
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        Assert.Empty(roleManager.Roles);
                        await roleManager.CreateAsync(new IdentityRole("TestRoleName"));
                        Assert.Single(roleManager.Roles);
                        var target = new UserRepository(genericRepository, userManager);

                        var result = await target.CreateAsync("TestUserName", "Password123", "user@example.com", "TestRoleName", null);

                        Assert.Single(userManager.Users);
                        Assert.Single(db.DomainUsers);
                        Assert.Equal("TestUserName", result.Identity.UserName);
                        Assert.Equal("user@example.com", result.Identity.Email);
                        Assert.True(string.IsNullOrWhiteSpace(result.Profile.FirstName));
                        Assert.True(string.IsNullOrWhiteSpace(result.Profile.LastName));
                        Assert.True(string.IsNullOrWhiteSpace(result.Profile.JobPosition));
                        Assert.True(string.IsNullOrWhiteSpace(result.Profile.PhotoFilePath));
                        Assert.False(result.Profile.IsReadyForDisplay);
                        Assert.False(result.Profile.DisplayAsTeamMember);
                        Assert.Equal(4, result.Profile.SocialLinks.Count);
                        Assert.True(userManager.IsInRoleAsync(result.Identity as UserIdentity, "TestRoleName").Result);
                        Assert.True(userManager.HasPasswordAsync(result.Identity as UserIdentity).Result);
                    }
                }
            }
        }

        // EfUserRepository.UpdateProfileAsync()
        // EfRepository.UpdateAsync()
        [Fact]
        public async Task UpdateProfileAsync_Good()
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
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());

                        var modifiedUser = testUsers.First();
                        modifiedUser.Profile.UpdatePersonalInfo("New FirstName", "New LastName", "New Job", "New Path");
                        modifiedUser.Profile.AddSocialLinks(new SocialLink("Facebook", "New Url"),
                            new SocialLink("Test Name", "Test Url"));
                        var target = new UserRepository(genericRepository, userManager);
                        int expectedSocialLinksCount = 5;

                        await target.UpdateProfileAsync(modifiedUser);

                        var result = await db.DomainUsers.FindAsync(modifiedUser.Id);
                        Assert.NotNull(result);
                        Assert.Equal(modifiedUser.Id, result.Id);
                        Assert.Equal("user1", result.Identity.UserName);
                        Assert.Equal("New FirstName", result.Profile.FirstName);
                        Assert.Equal("New LastName", result.Profile.LastName);
                        Assert.Equal("New Job", result.Profile.JobPosition);
                        Assert.Equal("New Path", result.Profile.PhotoFilePath);
                        Assert.True(result.Profile.IsReadyForDisplay);
                        Assert.True(result.Profile.DisplayAsTeamMember);
                        Assert.Equal(expectedSocialLinksCount, result.Profile.SocialLinks.Count);
                        Assert.Equal("Facebook", result.Profile.SocialLinks.First().NetworkName);
                        Assert.Equal("New Url", result.Profile.SocialLinks.First().Url);
                        Assert.Equal("Test Name", result.Profile.SocialLinks.Last().NetworkName);
                        Assert.Equal("Test Url", result.Profile.SocialLinks.Last().Url);
                    }
                }
            }
        }

        // EfUserRepository.DeleteAsync()
        // EfRepository.DeleteAsync()
        [Fact]
        public async Task DeleteAsync_Good()
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
                        var genericRepository = new Repository<ApplicationDbContext, DomainUser, int>(db);
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                        Assert.Empty(db.DomainUsers);
                        Assert.Empty(userManager.Users);
                        TestHelper.PopulateTestUsersAsync(userManager, testUsers).Wait();//
                        TestHelper.SeedDatabaseWithTestData(db, testUsers);
                        Assert.Equal(testUsers.Count(), db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count(), userManager.Users.Count());

                        var userToDelete = testUsers.Last();
                        var target = new UserRepository(genericRepository, userManager);

                        await target.DeleteAsync(userToDelete);

                        Assert.Equal(testUsers.Count() - 1, db.DomainUsers.Count());
                        Assert.Equal(testUsers.Count() - 1, userManager.Users.Count());
                        Assert.DoesNotContain(userToDelete, db.DomainUsers);
                        Assert.DoesNotContain(userToDelete.Identity as UserIdentity, userManager.Users);
                    }
                }
            }
        }

        private IEnumerable<DomainUser> GetTestUserCollection()
        {
            var users = new DomainUser[]
            {
                new DomainUser(new UserIdentity("user1", "user1@example.com"), new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1")),
                new DomainUser(new UserIdentity("user2", "user2@example.com"), new UserProfile("FirstName #2", "LastName #2", "Job #2", "Path #2")),
                new DomainUser(new UserIdentity("user3", "user3@example.com"), new UserProfile("FirstName #3", "LastName #3", "Job #3", "Path #3"))
            };

            foreach (var user in users)
            {
                user.Profile.AddSocialLinks(GetTestSocialLinks());
                user.Profile.ChangeDisplayStatus(true);
            }
            return users;
        }

        private SocialLink[] GetTestSocialLinks()
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
