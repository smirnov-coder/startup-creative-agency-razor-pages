using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Services
{
    public class ContactsServiceTests
    {
        private Mock<IFileService> _mockFileService = new Mock<IFileService>();
        private IContactsService _target;

        public ContactsServiceTests()
        {
            _target = new ContactsService(_mockFileService.Object);
        }

        [Fact]
        public async Task GetContactsAsync_Bad_DomainServiceException()
        {
            _mockFileService.Setup(x => x.GetFileContentAsStringAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.GetContactsAsync());

            Assert.Equal("Unable to get contacts. See inner exception for details.", ex.Message);
        }

        [Fact]
        public async Task SaveContactsAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.SaveContactsAsync(null));
        }

        [Fact]
        public async Task SaveContactsAsync_Bad_DomainServiceException()
        {
            _mockFileService.Setup(x => x.SaveTextDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.SaveContactsAsync(new List<ContactInfo>()));

            Assert.Equal("Unable to save contacts. See inner exception for details.", ex.Message);
        }

        [Fact]
        public async Task GetSocialLinksAsync_Bad_DomainServiceException()
        {
            _mockFileService.Setup(x => x.GetFileContentAsStringAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.GetSocialLinksAsync());

            Assert.Equal("Unable to get social links. See inner exception for details.", ex.Message);
        }

        [Fact]
        public async Task SaveSocialLinksAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.SaveSocialLinksAsync(null));
        }

        [Fact]
        public async Task SaveSocialLinksAsync_Bad_DomainServiceException()
        {
            _mockFileService.Setup(x => x.SaveTextDataAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.SaveSocialLinksAsync(new Dictionary<string, string>()));

            Assert.Equal("Unable to save social links. See inner exception for details.", ex.Message);
        }
    }
}
