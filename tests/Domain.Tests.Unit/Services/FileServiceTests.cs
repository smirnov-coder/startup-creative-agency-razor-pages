using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Logic.Services;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Services
{
    public class FileServiceTests
    {
        private Mock<IFileRepository> _mockFileRepository = new Mock<IFileRepository>();
        private IFileService _target;

        public FileServiceTests()
        {
            _target = new FileService(_mockFileRepository.Object);
        }

        [Theory]
        [InlineData(null), InlineData(""), InlineData(" ")]
        public async Task GetFileContentAsStringAsync_Bad_ArgumentException(string fileName)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.GetFileContentAsStringAsync(fileName));

            Assert.StartsWith("File name cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task GetFileContentAsStringAsync_Bad_DomainServiceException()
        {
            _mockFileRepository.Setup(x => x.GetFileByFileName(It.IsAny<string>())).Throws<Exception>();

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.GetFileContentAsStringAsync("test"));
            Assert.Equal("Unable to read text data from file 'test'. See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Theory]
        [InlineData(null, null), InlineData("", null), InlineData(" ", null)]
        public async Task SaveImageAsync_Bad_ArgumentException(string fileName, Stream imageStream)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.SaveImageAsync(fileName, imageStream));

            Assert.StartsWith("File name cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task SaveImageAsync_Bad_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _target.SaveImageAsync("test", null));
        }

        [Fact]
        public async Task SaveImageAsync_Bad_DomainServiceException()
        {
            _mockFileRepository.Setup(x => x.SaveImageAsync(It.IsAny<string>(), It.IsAny<Stream>())).ThrowsAsync(new Exception());

            using (var stream = new MemoryStream())
            {
                var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.SaveImageAsync("test", stream));
                Assert.Equal("Unable to save image 'test'. See inner exception for details.", ex.Message);
                Assert.NotNull(ex.InnerException);
            }
        }

        [Theory]
        [InlineData(null, "a")]
        [InlineData("", "a")]
        [InlineData(" ", "a")]
        public async Task SaveTextDataAsync_Bad_ArgumentException(string fileName, string textData)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.SaveTextDataAsync(fileName, textData));

            Assert.StartsWith("File name cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task SaveTextDataAsync_Bad_DomainServiceException()
        {
            _mockFileRepository.Setup(x => x.SaveTextDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Encoding>()))
                .ThrowsAsync(new Exception());

            var ex = await Assert.ThrowsAsync<DomainServiceException>(() => _target.SaveTextDataAsync("test", "test"));
            Assert.Equal("Unable to save text data to file 'test'. See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Theory]
        [InlineData(""), InlineData(" "), InlineData(null)]
        public void GenerateUniqueFileName_Bad_ArgumentException(string extension)
        {
            var ex = Assert.Throws<ArgumentException>(() => _target.GenerateUniqueFileName(extension));

            Assert.StartsWith("File extension cannot be null or empty string.", ex.Message);
        }
    }
}
