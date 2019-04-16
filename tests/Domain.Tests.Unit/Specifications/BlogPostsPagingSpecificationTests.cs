using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Specifications
{
    public class BlogPostsPagingSpecificationTests
    {
        [Fact]
        public void ValidSkipTake()
        {
            int skip = 1, take = 2;

            var target = new BlogPostsPagingSpecification(skip, take);

            Assert.True(target.IsPagingEnabled);
            Assert.Null(target.OrderBy);
            Assert.NotNull(target.OrderByDescending);
            var result = GetTestBlogPostCollection()
                .AsQueryable()
                .Where(target.Criteria)
                .OrderByDescending(target.OrderByDescending)
                .Skip(target.Skip)
                .Take(target.Take)
                .ToList();
            Assert.Equal(take, result.Count);
            Assert.Equal(103, result.First().Id);
            Assert.Equal(102, result.Last().Id);
            Assert.True(result.First().CreatedOn > result.Last().CreatedOn);
        }

        [Fact]
        public void SkipExceededValidTake()
        {
            int skip = 4, take = 2;
            var target = new BlogPostsPagingSpecification(skip, take);

            var result = GetTestBlogPostCollection()
                .AsQueryable()
                .Where(target.Criteria)
                .OrderByDescending(target.OrderByDescending)
                .Skip(target.Skip)
                .Take(target.Take)
                .ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void ValidSkipTakeExceeded()
        {
            int skip = 2, take = 4;
            var target = new BlogPostsPagingSpecification(skip, take);

            var result = GetTestBlogPostCollection()
                .AsQueryable()
                .Where(target.Criteria)
                .OrderByDescending(target.OrderByDescending)
                .Skip(target.Skip)
                .Take(target.Take)
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(102, result.First().Id);
            Assert.Equal(101, result.Last().Id);
        }

        private IList<BlogPost> GetTestBlogPostCollection()
        {
            var creator = new DomainUser(new UserIdentity());
            var blogPosts = new List<BlogPost>();
            for (int i = 1; i < 5; i++)
            {
                blogPosts.Add(new BlogPost(int.Parse($"10{i}"), creator)
                {
                    Title = $"Title #{i}",
                    ImagePath = $"Path #{i}",
                    Category = $"Category #{i}",
                    Content = $"Content #{i}"
                });
                Thread.Sleep(100);
            }
            return blogPosts;
        }
    }
}
