using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Integration.Services
{
    public class BlogServiceTests : IDisposable
    {
        private string _connectionString = "DataSource=:memory:";
        private SqliteConnection _connection;

        [Fact]
        public async Task GetBlogPostsAsync_Good_PageLoad()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                var testUsersCount = GetTestUserCollection().Count;
                var testBlogPosts = GetTestBlogPostCollection();
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    db.Database.EnsureCreated();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(db.BlogPosts);
                    TestHelper.SeedDatabaseWithTestData(db, testBlogPosts);
                }
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    Assert.Equal(testUsersCount, db.DomainUsers.Count());
                    Assert.Equal(testBlogPosts.Count, db.BlogPosts.Count());
                    var repository = new Repository<ApplicationDbContext, BlogPost, int>(db);
                    var target = new BlogService(repository);
                    int defaultSkip = 0, defaultTake = 2;

                    var result = await target.GetBlogPostsAsync(defaultSkip, defaultTake);

                    Assert.IsAssignableFrom<IList<BlogPost>>(result);
                    Assert.Equal(defaultTake, result.Count);
                    Assert.Equal("UserName #5", result.First().CreatedBy.Identity.UserName);
                    Assert.Equal("UserName #5", result.First().LastUpdatedBy.Identity.UserName);
                    Assert.True(result.First().LastUpdatedOn >= result.First().CreatedOn);
                    Assert.Equal("Title #5", result.First().Title);
                    Assert.Equal("Category #5", result.First().Category);
                    Assert.Equal("Path #5", result.First().ImagePath);
                    Assert.Equal("Content #5", result.First().Content);
                    Assert.Equal("UserName #4", result.Last().CreatedBy.Identity.UserName);
                    Assert.Equal("UserName #4", result.Last().LastUpdatedBy.Identity.UserName);
                    Assert.True(result.Last().LastUpdatedOn >= result.Last().CreatedOn);
                    Assert.Equal("Title #4", result.Last().Title);
                    Assert.Equal("Category #4", result.Last().Category);
                    Assert.Equal("Path #4", result.Last().ImagePath);
                    Assert.Equal("Content #4", result.Last().Content);
                }
            }
        }

        [Fact]
        public async Task GetBlogPostsAsync_Good_AjaxRequest()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                var testUsersCount = GetTestUserCollection().Count;
                var testBlogPosts = GetTestBlogPostCollection();
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    db.Database.EnsureCreated();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(db.BlogPosts);
                    TestHelper.SeedDatabaseWithTestData(db, testBlogPosts);
                }
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    Assert.Equal(testUsersCount, db.DomainUsers.Count());
                    Assert.Equal(testBlogPosts.Count, db.BlogPosts.Count());
                    var repository = new Repository<ApplicationDbContext, BlogPost, int>(db);
                    var target = new BlogService(repository);
                    int skip = 2, take = 3;

                    var result = await target.GetBlogPostsAsync(skip, take);

                    Assert.Equal(take, result.Count);
                    Assert.Equal("UserName #3", result.First().CreatedBy.Identity.UserName);
                    Assert.Equal("UserName #3", result.First().LastUpdatedBy.Identity.UserName);
                    Assert.True(result.First().LastUpdatedOn >= result.First().CreatedOn);
                    Assert.Equal("Title #3", result.First().Title);
                    Assert.Equal("Category #3", result.First().Category);
                    Assert.Equal("Path #3", result.First().ImagePath);
                    Assert.Equal("Content #3", result.First().Content);
                    Assert.Equal("UserName #1", result.Last().CreatedBy.Identity.UserName);
                    Assert.Equal("UserName #1", result.Last().LastUpdatedBy.Identity.UserName);
                    Assert.True(result.Last().LastUpdatedOn >= result.Last().CreatedOn);
                    Assert.Equal("Title #1", result.Last().Title);
                    Assert.Equal("Category #1", result.Last().Category);
                    Assert.Equal("Path #1", result.Last().ImagePath);
                    Assert.Equal("Content #1", result.Last().Content);

                }
            }
        }

        [Theory]
        [InlineData(3), InlineData(4)]
        public async Task GetBlogPostsAsync_Good_TakeMoreThanOrEqualFetchThreshold(int take)
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                var testUsersCount = GetTestUserCollection().Count;
                var testBlogPosts = GetTestBlogPostCollection();
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    db.Database.EnsureCreated();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(db.BlogPosts);
                    TestHelper.SeedDatabaseWithTestData(db, testBlogPosts);
                    Assert.Equal(testUsersCount, db.DomainUsers.Count());
                    Assert.Equal(testBlogPosts.Count, db.BlogPosts.Count());
                    var repository = new Repository<ApplicationDbContext, BlogPost, int>(db);
                    var target = new BlogService(repository)
                    {
                        FetchThreshold = 3
                    };
                    int skip = 0;

                    var result = await target.GetBlogPostsAsync(skip, take);

                    Assert.Equal(target.FetchThreshold, result.Count);
                }
            }
        }

        [Fact]
        public async Task GetBlogPostsAsync_Good_TakeMoreThanObjectsCount()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                var testUsersCount = GetTestUserCollection().Count;
                var testBlogPosts = GetTestBlogPostCollection();
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    db.Database.EnsureCreated();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(db.BlogPosts);
                    TestHelper.SeedDatabaseWithTestData(db, testBlogPosts);
                    Assert.Equal(testUsersCount, db.DomainUsers.Count());
                    Assert.Equal(testBlogPosts.Count, db.BlogPosts.Count());
                    var repository = new Repository<ApplicationDbContext, BlogPost, int>(db);
                    var target = new BlogService(repository);
                    int skip = 0, take = 6;

                    var result = await target.GetBlogPostsAsync(skip, take);

                    Assert.Equal(testBlogPosts.Count, result.Count);
                }
            }
        }

        [Theory]
        [InlineData(5), InlineData(6)]
        public async Task GetBlogPostsAsync_Good_SkipMoreThanOrEqualObjectsCount(int skip)
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                var testUsersCount = GetTestUserCollection().Count;
                var testBlogPosts = GetTestBlogPostCollection();
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    db.Database.EnsureCreated();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(db.BlogPosts);
                    TestHelper.SeedDatabaseWithTestData(db, testBlogPosts);
                    Assert.Equal(testUsersCount, db.DomainUsers.Count());
                    Assert.Equal(testBlogPosts.Count, db.BlogPosts.Count());
                    var repository = new Repository<ApplicationDbContext, BlogPost, int>(db);
                    var target = new BlogService(repository);
                    int take = 2;

                    var result = await target.GetBlogPostsAsync(skip, take);

                    Assert.NotNull(result);
                    Assert.Empty(result);
                }
            }
        }

        [Fact]
        public async Task UpdateBlogPostAsync_Good()
        {
            using (_connection = new SqliteConnection(_connectionString))
            {
                await _connection.OpenAsync();
                var testUsers = GetTestUserCollection();
                var testBlogPosts = GetTestBlogPostCollection();
                using (var db = new ApplicationDbContext(TestHelper.CreateTestDbContextOptions(_connection)))
                {
                    db.Database.EnsureCreated();
                    Assert.Empty(db.DomainUsers);
                    Assert.Empty(db.BlogPosts);
                    TestHelper.SeedDatabaseWithTestData(db, testBlogPosts);
                    Assert.Equal(testUsers.Count, db.DomainUsers.Count());
                    Assert.Equal(testBlogPosts.Count, db.BlogPosts.Count());
                    var repository = new Repository<ApplicationDbContext, BlogPost, int>(db);
                    var target = new BlogService(repository);
                    var updatedBy = testUsers.First();
                    var modifiedBlogPost = testBlogPosts.Last();
                    var createdBy = modifiedBlogPost.CreatedBy;
                    var createdOn = modifiedBlogPost.CreatedOn;
                    modifiedBlogPost.Title = "New Title";
                    modifiedBlogPost.Category = "New Category";
                    modifiedBlogPost.ImagePath = "New Path";
                    modifiedBlogPost.Content = "New Content";
                    modifiedBlogPost.LastUpdatedBy = updatedBy;
                    modifiedBlogPost.LastUpdatedOn = new DateTime(2020, 1, 1);

                    await target.UpdateBlogPostAsync(modifiedBlogPost);

                    var result = await db.BlogPosts.FindAsync(modifiedBlogPost.Id);
                    Assert.NotNull(result);
                    Assert.Equal(createdBy.Id, result.CreatedBy.Id);
                    Assert.Equal(createdOn, result.CreatedOn);
                    Assert.Equal(updatedBy.Id, result.LastUpdatedBy.Id);
                    Assert.Equal(new DateTime(2020, 1, 1), result.LastUpdatedOn);
                    Assert.Equal("New Title", result.Title);
                    Assert.Equal("New Category", result.Category);
                    Assert.Equal("New Path", result.ImagePath);
                    Assert.Equal("New Content", result.Content);
                }
            }
        }

        private IList<BlogPost> GetTestBlogPostCollection()
        {
            var users = GetTestUserCollection();
            var blogPosts = new List<BlogPost>();
            for (int i = 0; i < 5; i++)
            {
                blogPosts.Add(new BlogPost(int.Parse($"10{i + 1}"), users[i])
                {
                    Title = $"Title #{i + 1}",
                    Category = $"Category #{i + 1}",
                    ImagePath = $"Path #{i + 1}",
                    Content = $"Content #{i + 1}"
                });
                Thread.Sleep(100);
            }
            return blogPosts;
        }

        private IList<DomainUser> GetTestUserCollection()
        {
            return new List<DomainUser>
            {
                new DomainUser(new UserIdentity("UserName #1", null)),
                new DomainUser(new UserIdentity("UserName #2", null)),
                new DomainUser(new UserIdentity("UserName #3", null)),
                new DomainUser(new UserIdentity("UserName #4", null)),
                new DomainUser(new UserIdentity("UserName #5", null))
            };
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}