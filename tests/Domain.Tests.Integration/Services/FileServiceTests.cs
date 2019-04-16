using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Integration.Services
{
    [Collection("TestFileCollection")]
    public class FileServiceTests
    {
        private TestFiles _testFiles;
        private Mock<IHostingEnvironment> _mockEnvironment = new Mock<IHostingEnvironment>();
        private IFileRepository _fileRepository;
        private IFileService _target;

        public FileServiceTests(TestFiles testFiles)
        {
            _testFiles = testFiles;
            _mockEnvironment.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _mockEnvironment.Setup(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _fileRepository = new FileRepository(_mockEnvironment.Object);
            _target = new FileService(_fileRepository);
        }

        [Fact]
        public async Task GetFileContentAsStringAsync_Good()
        {
            var result = await _target.GetFileContentAsStringAsync(_testFiles.ReadTextFileName);

            Assert.Equal("dummy text", result);
        }

        [Fact]
        public async Task SaveImageAsync_Good()
        {
            using (var stream = new MemoryStream(new byte[1024]))
            {
                Assert.False(File.Exists(_testFiles.WriteImageFilePath));

                var result = await _target.SaveImageAsync(_testFiles.WriteImageFileName, stream);

                Assert.True(File.Exists(_testFiles.WriteImageFilePath));
                Assert.True(new FileInfo(_testFiles.WriteImageFilePath).Length > 0);
                Assert.Equal("~/images/test-write.jpg", result);
            }
        }

        [Fact]
        public async Task SaveTextDataAsync_Good()
        {
            Assert.False(File.Exists(_testFiles.WriteTextFilePath));

            await _target.SaveTextDataAsync(_testFiles.WriteTextFileName, "test");

            Assert.True(File.Exists(_testFiles.WriteTextFilePath));
            Assert.Equal("test", File.ReadAllText(_testFiles.WriteTextFilePath));
        }
    }
}
