using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Tests.Shared;
using Xunit;

namespace StartupCreativeAgency.Infrastructure.Tests.Integration.Repositories
{
    [Collection("TestFileCollection")]
    public class FileRepositoryTests
    {
        private Mock<IHostingEnvironment> _mockEnvironment = new Mock<IHostingEnvironment>();
        private TestFiles _testFiles;
        private IFileRepository _target;

        public FileRepositoryTests(TestFiles testFiles)
        {
            _testFiles = testFiles;
            _mockEnvironment.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _mockEnvironment.Setup(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _target = new FileRepository(_mockEnvironment.Object);
        }

        [Fact]
        public async Task GetFileByFileName_Good()
        {
            var result = _target.GetFileByFileName(_testFiles.ReadTextFileName);

            Assert.NotNull(result);
            string text = string.Empty;
            using (var reader = result.OpenText())
            {
                text = await reader.ReadToEndAsync();
            }
            Assert.Equal("dummy text", text);
        }

        [Fact]
        public async Task SaveTextDataAsync_Good()
        {
            await _target.SaveTextDataAsync(_testFiles.WriteTextFileName, "test text");

            Assert.True(File.Exists(_testFiles.WriteTextFilePath));
            string text = File.ReadAllText(_testFiles.WriteTextFilePath);
            Assert.Equal("test text", text);
        }

        [Fact]
        public async Task SaveImageAsync_Good()
        {
            using (var stream = new MemoryStream())
            {
                byte[] content = new byte[1024];
                Array.Fill<byte>(content, 255);
                await stream.WriteAsync(content);
                string result = await _target.SaveImageAsync(_testFiles.WriteImageFileName, stream);

                Assert.Equal($"~/images/{_testFiles.WriteImageFileName}", result);
                Assert.True(File.Exists(_testFiles.WriteImageFilePath));
                Assert.Equal(content.Length, new FileInfo(_testFiles.WriteImageFilePath).Length);
            }
        }
    }
}
