using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class MessagesModelTests
    {
        private Mock<IMessageService> _mockMessageService = new Mock<IMessageService>();
        private MessagesModel _target;

        public MessagesModelTests()
        {
            _target = new MessagesModel(_mockMessageService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task OnPostUnreadAsync_Good()
        {
            var testModel = GetTestModel();

            var actionResult = await _target.OnPostUnreadAsync(testModel);

            _mockMessageService.Verify(x => x.UpdateMessageReadStatusAsync(It.IsAny<int>(), false), Times.Exactly(2));
            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/MESSAGES", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal($"A set of entities of type '{typeof(Message)}' has been updated successfully.", details.Message);
        }

        [Fact]
        public async Task OnPostDeleteAsync_Good()
        {
            var testModel = GetTestModel();

            var actionResult = await _target.OnPostDeleteAsync(testModel);

            _mockMessageService.Verify(x => x.DeleteMessageAsync(It.IsAny<int>()), Times.Exactly(2));
            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/MESSAGES", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal($"A set of entities of type '{typeof(Message)}' has been deleted successfully.", details.Message);
        }

        [Fact]
        public async Task OnPostDeleteAsync_Bad()
        {
            var testModel = GetTestModel();
            _target.ModelState.AddModelError("", "Test Error");

            var actionResult = await _target.OnPostDeleteAsync(testModel);

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/MESSAGES", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Error, details.Status);
            Assert.Equal("Unable to perform operation. Reload the page and try again.", details.Message);
        }

        private MessageViewModel[] GetTestModel()
        {
            return new MessageViewModel[]
            {
                new MessageViewModel { Id = 101, IsRead = true },
                new MessageViewModel { Id = 102, IsRead = false },
                new MessageViewModel { Id = 103, IsRead = true }
            };
        }
    }
}
