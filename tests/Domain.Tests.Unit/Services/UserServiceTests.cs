using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Services
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepository = new Mock<IUserRepository>();
        private IUserService _target;

        public UserServiceTests()
        {
            _target = new UserService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetUsersAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<DomainUser>>()))
                .ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.GetUsersAsync());

            Assert.StartsWith("Unable to get entities of type", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Theory]
        [InlineData(""), InlineData(" "), InlineData(null)]
        public async Task GetUserAsync_Bad_ArgumentException(string userName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.GetUserAsync(userName));

            Assert.StartsWith("Argument value cannot be null or empty string.", ex.Message);
        }

        [Theory]
        [InlineData("", "a", "a", "a")]
        [InlineData("a", " ", "a", "a")]
        [InlineData("a", "a", null, "a")]
        [InlineData("a", "a", "a", "")]
        public async Task CreateUsersAsync_Bad_ArgumentException(string userName, string password, string email, string roleName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.CreateUserAsync(userName, password, email, roleName, null));

            Assert.StartsWith("Argument value cannot be null or empty string.", ex.Message);
        }

        private DomainUser _testUser = new DomainUser(new UserIdentity());

        [Fact]
        public async Task CreateUserAsync_Bad_DuplicateEntityException()
        {
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(_testUser);

            var ex = await Assert.ThrowsAsync<DuplicateEntityException>(() => 
                _target.CreateUserAsync("Test UserName", "Test Password", "Test Email", "Test Role"));

            Assert.EndsWith("with value 'Test UserName' for 'UserName' already exists.", ex.Message);
        }

        [Fact]
        public async Task CreateUserAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => 
                _target.CreateUserAsync("Test UserName", "Test Password", "Test Email", "Test Role"));

            Assert.StartsWith("Unable to create entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Theory]
        [InlineData(""), InlineData(" "), InlineData(null)]
        public async Task UpdatePersonalInfoAsync_Bad_ArgumentException(string userName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => 
                _target.UpdateUserPersonalInfoAsync(userName, "a", "b", "c", "d"));

            Assert.StartsWith("Argument value cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task UpdatePersonalInfoAsync_Bad_EntityNotFoundException()
        {
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<ISpecification<DomainUser>>()))
                .ReturnsAsync(default(DomainUser));

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
                _target.UpdateUserPersonalInfoAsync("a", "b", "c", "d", "e"));

            Assert.StartsWith("The entity of type", ex.Message);
            Assert.EndsWith("that you trying to update doesn't exist.", ex.Message);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public async Task UpdatePersonalInfoAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => 
                _target.UpdateUserPersonalInfoAsync("a", "b", "c", "d", "e"));

            Assert.StartsWith("Unable to update entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Theory]
        [InlineData(""), InlineData(" "), InlineData(null)]
        public async Task DeleteUserAsync_Bad_ArgumentException(string userName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.DeleteUserAsync(userName));

            Assert.StartsWith("Argument value cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task DeleteUserAsync_Bad_EntityNotFoundException()
        {
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<ISpecification<DomainUser>>()))
                .ReturnsAsync(default(DomainUser));

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _target.DeleteUserAsync("Test UserName"));

            Assert.StartsWith("The entity of type", ex.Message);
            Assert.EndsWith("with value 'Test UserName' for 'UserName' not found.", ex.Message);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public async Task DeleteUserAsync_Bad_DomainServiceException()
        {
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.DeleteUserAsync("Test UserName"));

            Assert.StartsWith("Unable to delete entity of type", ex.Message);
            Assert.EndsWith("See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }
    }
}
