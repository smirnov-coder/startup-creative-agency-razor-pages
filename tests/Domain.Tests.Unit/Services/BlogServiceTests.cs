using System;
using System.Threading.Tasks;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Services
{
    public class BlogServiceTests
    {
        private IBlogService _target = new BlogService(Mock.Of<IRepository<BlogPost, int>>());

        [Theory]
        [InlineData(-1, 0), InlineData(0, -1)]
        public async Task GetBlogPostsAsync_Bad_ArgumentOutOfRangeException(int count, int skip)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _target.GetBlogPostsAsync(count, skip));
        }

        [Fact]
        public void FetchThreshold_Bad_ArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _target.FetchThreshold = -1);

            Assert.StartsWith("Value cannot be less than 0.", ex.Message);
        }

    }
}
