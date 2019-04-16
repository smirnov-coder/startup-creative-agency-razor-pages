using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using StartupCreativeAgency.Domain.Abstractions;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;
using Xunit;

namespace StartupCreativeAgency.Infrastructure.Tests.Unit.Repositories
{
    public class UserRepositoryTests
    {
        private Mock<IRepository<DomainUser, int>> _mockRepository = new Mock<IRepository<DomainUser, int>>();
        private Mock<IUserStore<UserIdentity>> _mockStore = new Mock<IUserStore<UserIdentity>>();
        private Mock<UserManager<UserIdentity>> _mockUserManager;
        private ISpecification<DomainUser> _testSpecification = new BaseSpecification<DomainUser>(x => true);
        private DomainUser _testUser = new DomainUser(new UserIdentity("Test UserName", "Test Email"));
        private IUserRepository _target;

        public UserRepositoryTests()
        {
            _mockUserManager = new Mock<UserManager<UserIdentity>>(_mockStore.Object, 
                null, null, null, null, null, null, null, null);
            _mockRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<DomainUser>>()))
                .ThrowsAsync(new DataAccessException());
            _target = new UserRepository(_mockRepository.Object, _mockUserManager.Object);
        }

        [Theory]
        [InlineData(""), InlineData(" "), InlineData(null)]
        public async Task GetAsync_Bad_ArgumentException(string userName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.GetAsync(userName));
            Assert.StartsWith("Argument value cannot be null or empty string.", ex.Message);
        }

        [Theory]
        [InlineData("", "a", "a", "a")]
        [InlineData("a", " ", "a", "a")]
        [InlineData("a", "a", null, "a")]
        [InlineData("a", "a", "a", "")]
        public async Task CreateAsync_Bad_ArgumentException(string userName, string password, string email, string roleName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.CreateAsync(userName, password, email, roleName, null));
            Assert.StartsWith("Argument value cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Bad_CreateIdentityFailed_DataAccessException()
        {
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error #1" }, new IdentityError { Description = "Error #2" }));

            var result = await Assert.ThrowsAsync<DataAccessException>(() => 
                _target.CreateAsync("Test UserName", "Test Password", "Test Email", "Test Role", null));

            Assert.StartsWith("An error occurred while creating identity for user 'Test UserName'.", result.Message);
            Assert.EndsWith("Inspect 'Errors' property for details.", result.Message);
            Assert.Equal(2, result.Errors.Count());
            Assert.Equal("Error #1", result.Errors.FirstOrDefault());
            Assert.Equal("Error #2", result.Errors.LastOrDefault());
        }

        [Fact]
        public async Task CreateAsync_Bad_AddToRoleFailed_DataAccessException()
        {
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserIdentity("Test UserName", "Test Email"));
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<UserIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error #1" }, new IdentityError { Description = "Error #2" }));

            var result = await Assert.ThrowsAsync<DataAccessException>(() => 
                _target.CreateAsync("Test UserName", "Test Password", "Test Email", "Test Role", null));

            Assert.StartsWith("An error occurred while adding user 'Test UserName' to role 'Test Role'.", result.Message);
            Assert.EndsWith("Inspect 'Errors' property for details.", result.Message);
            Assert.Equal(2, result.Errors.Count());
            Assert.Equal("Error #1", result.Errors.FirstOrDefault());
            Assert.Equal("Error #2", result.Errors.LastOrDefault());
        }

        [Fact]
        public async Task CreateAsync_Bad_DbContextInteractionFailed_DataAccessException()
        {
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserIdentity());
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<UserIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<DomainUser>())).ThrowsAsync(new Exception());

            var result = await Assert.ThrowsAsync<DataAccessException>(() => 
                _target.CreateAsync("Test UserName", "Test Password", "Test Email", "Test Role", null));

            Assert.Equal($"Unable to create entity of type '{typeof(DomainUser)}'. See inner exception for details.", result.Message);
            Assert.NotNull(result.InnerException);
        }

        [Fact]
        public async Task DeleteAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.DeleteAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_Bad_DeleteIdentityFailed_DataAccessException()
        {
            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<UserIdentity>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error #1" }, new IdentityError { Description = "Error #2" }));

            var result = await Assert.ThrowsAsync<DataAccessException>(() => _target.DeleteAsync(_testUser));

            Assert.StartsWith("An error occurred while deleting identity for user 'Test UserName'.", result.Message);
            Assert.EndsWith("Inspect 'Errors' property for details.", result.Message);
            Assert.Equal(2, result.Errors.Count());
            Assert.Equal("Error #1", result.Errors.FirstOrDefault());
            Assert.Equal("Error #2", result.Errors.LastOrDefault());
        }

        [Fact]
        public async Task DeleteAsync_Bad_DeleteEntityFailed_DataAccessException()
        {
            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<UserIdentity>())).Throws<Exception>();

            var result = await Assert.ThrowsAsync<DataAccessException>(() => _target.DeleteAsync(_testUser));

            Assert.Equal($"Unable to delete entity of type '{typeof(DomainUser)}'. See inner exception for details.", result.Message);
        }
    }
}
