using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Exceptions;
using Xunit;

namespace StartupCreativeAgency.Infrastructure.Tests.Unit.Repositories
{
    public class FileRepositoryTests
    {
        private FileRepositoryUnderTest _target;

        public FileRepositoryTests()
        {
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            mockEnvironment.Setup(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _target = new FileRepositoryUnderTest(mockEnvironment.Object);
        }

        [Theory]
        [InlineData(null), InlineData(""), InlineData(" ")]
        public void GetFileByFileName_Bad_ArgumentException(string fileName)
        {
            var ex = Assert.Throws<ArgumentException>(() => _target.GetFileByFileName(fileName));
            Assert.StartsWith("File name cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public void GetFileByFileName_Bad_FileNotFoundException()
        {
            _target.IsFileExistReturns = false;
            var ex = Assert.Throws<FileNotFoundException>(() => _target.GetFileByFileName("test"));
            Assert.StartsWith("File 'test' not found in directory", ex.Message);
        }

        [Fact]
        public void GetFileByFileName_Bad_DataAccessException()
        {
            _target.DataDirectory = null;
            var ex = Assert.Throws<DataAccessException>(() => _target.GetFileByFileName("test"));
            Assert.Equal("Unable to get file 'test'. See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }

        [Theory]
        [InlineData("", null), InlineData(" ", null), InlineData(null, null)]
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
        public async Task SaveImageAsync_Bad_DataAccessException()
        {
            _target.ImagesDirectory = null;
            using (var stream = new MemoryStream())
            {
                var ex = await Assert.ThrowsAsync<DataAccessException>(() => _target.SaveImageAsync("test", stream));
                Assert.Equal("An error occurred while saving image data to file 'test'. See inner exception for details.", ex.Message);
                Assert.NotNull(ex.InnerException);
            }
        }

        [Theory]
        [InlineData("", "a"), InlineData(" ", "a"), InlineData(null, "a")]
        public async Task SaveTextDataAsync_Bad_ArgumentException(string fileName, string textData)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _target.SaveTextDataAsync(fileName, textData));
            Assert.StartsWith("File name cannot be null or empty string.", ex.Message);
        }

        [Fact]
        public async Task SaveTextDataAsync_Bad_DataAccessException()
        {
            _target.DataDirectory = null;
            var ex = await Assert.ThrowsAsync<DataAccessException>(() => _target.SaveTextDataAsync("test", "test"));
            Assert.StartsWith("An error occurred while saving text data to file 'test'. See inner exception for details.", ex.Message);
            Assert.NotNull(ex.InnerException);
        }
    }

    class FileRepositoryUnderTest : FileRepository
    {
        public FileRepositoryUnderTest(IHostingEnvironment environment) : base(environment)
        { }

        public bool IsFileExistReturns { get; set; }

        protected override bool IsFileExist(string filePath) => IsFileExistReturns;
    }
}

