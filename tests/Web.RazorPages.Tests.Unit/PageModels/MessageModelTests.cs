using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class MessageModelTests
    {
        private Mock<IMessageService> _mockMessageService = new Mock<IMessageService>();
        private MessageModel _target;

        public MessageModelTests()
        {
            _target = new MessageModel(_mockMessageService.Object);
        }

        [Fact]
        public async Task OnGetAsync_Good()
        {
            var testMessage = new Message();
            _mockMessageService.Setup(x => x.GetMessageAsync(testMessage.Id)).ReturnsAsync(testMessage);

            await _target.OnGetAsync(testMessage.Id);

            _mockMessageService.Verify(x => x.UpdateMessageReadStatusAsync(testMessage.Id, true), Times.Once());
            Assert.NotNull(_target.Message);
            Assert.Same(testMessage, _target.Message);
        }
    }
}
