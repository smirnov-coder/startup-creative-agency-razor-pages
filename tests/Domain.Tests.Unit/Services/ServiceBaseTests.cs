using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Services
{
    public class ServiceBaseTests
    {
        private Mock<IRepository<BaseEntity<int>, int>> _mockRepository = 
            new Mock<IRepository<BaseEntity<int>, int>>();
        private ServiceBaseUnderTest _target;
        private ISpecification<BaseEntity<int>> _testSpecification = new TestSpecification();

        public ServiceBaseTests()
        {
            _target = new ServiceBaseUnderTest(_mockRepository.Object);
        }

        [Fact]
        public async Task ListBySpecificationAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.ListAsync(null));
        }

        // ServiceBase.ListAsync(ISpecification)
        [Fact]
        public async Task ListAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<BaseEntity<int>>>())).Throws<Exception>();

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.ListAsync(_testSpecification));

            Assert.StartsWith("Unable to list entities of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task GetBySpecificationAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.GetBySpecificationAsync(null));
        }

        // ServiceBase.GetBySpecification(ISpecification)
        [Fact]
        public async Task GetByIdAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<BaseEntity<int>>>())).Throws<DataAccessException>();

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.GetBySpecificationAsync(_testSpecification));

            Assert.StartsWith("Unable to get entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task AddAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<BaseEntity<int>>())).Throws<Exception>();

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.AddAsync(new BaseEntity<int>(101)));

            Assert.StartsWith("Unable to add entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task AddAsync_Bad_DuplicateEntityException()
        {
            _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new BaseEntity<int>(101));

            var ex = await Assert.ThrowsAsync<DuplicateEntityException>(() => _target.AddAsync(new BaseEntity<int>(101)));

            Assert.EndsWith("with key value '101' for 'Id' is already exists. If you want to update it, use 'Update' method", ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.UpdateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_Bad_DomainServiceException()
        {
            var testEntity = new BaseEntity<int>(101);
            _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<BaseEntity<int>>())).Throws<DataAccessException>();
            _mockRepository.Setup(x => x.GetByIdAsync(testEntity.Id)).ReturnsAsync(testEntity);

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.UpdateAsync(testEntity));

            Assert.StartsWith("Unable to update entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Fact]
        public async Task UpdateAsync_Bad_EntityNotFoundException()
        {
            _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(default(BaseEntity<int>));

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _target.UpdateAsync(new BaseEntity<int>(101)));

            Assert.EndsWith("with key value '101' for 'Id' that you trying to update doesn't exist. To add new entity, use 'Add' method.",
                ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_Bad_EntityNotFoundException()
        {
            _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(default(BaseEntity<int>));

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _target.DeleteAsync(101));

            Assert.EndsWith("with key value '101' for 'Id' not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_Bad_DomainServiceException()
        {
            var testEntity = new BaseEntity<int>(101);
            _mockRepository.Setup(x => x.DeleteAsync(testEntity)).Throws<DataAccessException>();
            _mockRepository.Setup(x => x.GetByIdAsync(testEntity.Id)).ReturnsAsync(testEntity);

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.DeleteAsync(testEntity.Id));

            Assert.StartsWith("Unable to delete entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }
    }

    internal class ServiceBaseUnderTest : ServiceBase<BaseEntity<int>, int>
    {
        public ServiceBaseUnderTest(IRepository<BaseEntity<int>, int> repository) 
            : base(repository) { }

        public new async Task<IList<BaseEntity<int>>> ListAsync(ISpecification<BaseEntity<int>> specification)
        {
            return await base.ListAsync(specification);
        }

        public new async Task<BaseEntity<int>> GetBySpecificationAsync(ISpecification<BaseEntity<int>> specification)
        {
            return await base.GetBySpecificationAsync(specification);
        }

        public new async Task<BaseEntity<int>> AddAsync(BaseEntity<int> entity)
        {
            return await base.AddAsync(entity);
        }

        public new async Task UpdateAsync(BaseEntity<int> entity)
        {
            await base.UpdateAsync(entity);
        }

        public new async Task DeleteAsync(int entityId)
        {
            await base.DeleteAsync(entityId);
        }

        protected override BaseEntity<int> UpdateExistingEntity(BaseEntity<int> existingEntity, BaseEntity<int> newEntity)
        {
            return existingEntity;
        }
    }
}
