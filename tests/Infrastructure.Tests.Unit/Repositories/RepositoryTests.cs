using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using Xunit;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Tests.Shared;

namespace StartupCreativeAgency.Infrastructure.Tests.Unit.Repositories
{
    public class RepositoryTests
    {
        private Mock<DbSet<BaseEntity<int>>> _mockSet = new Mock<DbSet<BaseEntity<int>>>();
        private Mock<ApplicationDbContext> _mockDbContext = new Mock<ApplicationDbContext>();
        private IRepository<BaseEntity<int>, int> _target;
        private ISpecification<BaseEntity<int>> _testSpecification = new TestSpecification();

        public RepositoryTests()
        {
            _mockSet.As<IQueryable<BaseEntity<int>>>().Setup(m => m.GetEnumerator()).Throws<Exception>();
            _mockDbContext.Setup(x => x.Set<BaseEntity<int>>()).Returns(_mockSet.Object);
            _target = new Repository<ApplicationDbContext, BaseEntity<int>, int>(_mockDbContext.Object);
        }

        // Repository.ListAsync(ISpecification)
        [Fact]
        public async Task ListAsync_Bad_DataAccessException()
        {
            var ex = await Assert.ThrowsAsync<DataAccessException>(() => _target.ListAsync());
            Assert.StartsWith("Unable to list entities of type", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task ListBySpecificationAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.ListAsync(null));
        }

        [Fact]
        public async Task GetBySpecificationAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.GetBySpecificationAsync(null));
        }

        // Repository.GetBySpecificationAsync()
        [Fact]
        public async Task GetByIdAsync_Bad_DataAccessException()
        {
            await Assert.ThrowsAsync<DataAccessException>(() => _target.GetByIdAsync(101));
        }

        [Fact]
        public async Task AddAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_Bad_DataAccessException()
        {
            var ex = await Assert.ThrowsAsync<DataAccessException>(() => _target.AddAsync(new BaseEntity<int>(101)));
            Assert.StartsWith("Unable to add entity of type", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task UpdateAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.UpdateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_Bad_DataAccessException()
        {
            var ex = await Assert.ThrowsAsync<DataAccessException>(() => _target.UpdateAsync(new BaseEntity<int>(101)));
            Assert.StartsWith("Unable to update entity of type", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task DeleteAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.DeleteAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_Bad_DataAccessException()
        {
            _mockDbContext.Setup(x => x.Set<BaseEntity<int>>().Remove(It.IsAny<BaseEntity<int>>())).Throws<Exception>();
            _target = new Repository<ApplicationDbContext, BaseEntity<int>, int>(_mockDbContext.Object);

            var ex = await Assert.ThrowsAsync<DataAccessException>(() => _target.DeleteAsync(new BaseEntity<int>(101)));

            Assert.StartsWith("Unable to delete entity of type", ex.Message);
            Assert.NotNull(ex.InnerException);
        }
    }
}
