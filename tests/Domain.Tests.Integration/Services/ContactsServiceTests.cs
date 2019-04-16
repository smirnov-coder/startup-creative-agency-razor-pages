using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Newtonsoft.Json;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Integration.Services
{
    [Collection("TestFileCollection")]
    public class ContactsServiceTests
    {
        private Mock<IHostingEnvironment> _mockEnvironment = new Mock<IHostingEnvironment>();
        private TestFiles _testFiles;
        private IFileRepository _fileRepository;
        private IFileService _fileService;
        private ContactsService _target;

        public ContactsServiceTests(TestFiles testFiles)
        {
            _testFiles = testFiles;
            _mockEnvironment.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _mockEnvironment.Setup(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _fileRepository = new FileRepository(_mockEnvironment.Object);
            _fileService = new FileService(_fileRepository);
            _target = new ContactsService(_fileService);
        }

        [Fact]
        public async Task GetContactsAsync_Good()
        {
            _target.ContactsFileName = "read-contacts.json";
            int expectedContactCount = 3;

            var result = await _target.GetContactsAsync();

            Assert.IsAssignableFrom<IList<ContactInfo>>(result);
            Assert.Equal(expectedContactCount, result.Count);
            Assert.Equal("Name #1", result.First().Name);
            Assert.Equal("Caption #1", result.First().Caption);
            Assert.Equal(3, result.First().Values.Count);
            Assert.Equal("Name #3", result.Last().Name);
            Assert.Equal("Caption #3", result.Last().Caption);
            Assert.Equal(2, result.Last().Values.Count);
        }

        [Fact]
        public async Task SaveContactsAsync_Good()
        {
            var testContacts = new ContactInfo[]
            {
                new ContactInfo("Test Name") { Caption = "Test Caption", Values = new string[] { "Test Value" }}
            };
            _target.ContactsFileName = "write-contacts.json";

            await _target.SaveContactsAsync(testContacts);

            var result = JsonConvert.DeserializeObject<List<ContactInfo>>(File.ReadAllText(_testFiles.WriteContactsFilePath));

            Assert.Single(result);
            Assert.Equal("Test Name", result.First().Name);
            Assert.Equal("Test Caption", result.First().Caption);
            Assert.Single(result.First().Values);
            Assert.Equal("Test Value", result.First().Values.First());
        }

        [Fact]
        public async Task GetSocialLinksAsync_Good()
        {
            _target.SocialLinksFileName = "read-social-links.json";
            int expectedSocialLinkCount = 3;

            var result = await _target.GetSocialLinksAsync();

            Assert.IsAssignableFrom<IDictionary<string, string>>(result);
            Assert.Equal(expectedSocialLinkCount, result.Count);
            Assert.Equal("Url #1", result["Name #1"]);
            Assert.Equal("Url #2", result["Name #2"]);
            Assert.Equal("Url #3", result["Name #3"]);
        }

        [Fact]
        public async Task SaveSocialLinksAsync_Good()
        {
            var testSocialLinks = new Dictionary<string, string>
            {
                ["Test Name"] = "Test Url"
            };
            _target.SocialLinksFileName = "write-social-links.json";

            await _target.SaveSocialLinksAsync(testSocialLinks);

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_testFiles.WriteSocialLinksFilePath));

            Assert.Single(result);
            Assert.Equal("Test Url", result["Test Name"]);
        }
    }
}
