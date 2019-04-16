using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Users;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class ManageModelTests
    {
        private Mock<IUserService> _mockUserService = new Mock<IUserService>();
        private ManageModel _target;

        public ManageModelTests()
        {
            _target = new ManageModel(_mockUserService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task OnGetAsync_Good()
        {
            var testUser = new DomainUser(new UserIdentity());
            _mockUserService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(testUser);

            var actionResult = await _target.OnGetAsync("Test UserName");

            Assert.IsAssignableFrom<PageResult>(actionResult);
            Assert.Equal(testUser, _target.DomainUser);
        }

        [Fact]
        public async Task OnGetAsync_Bad_NotFound()
        {
            _mockUserService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(default(DomainUser));

            var actionResult = await _target.OnGetAsync("Test UserName");

            Assert.IsAssignableFrom<NotFoundResult>(actionResult);
        }


        [Fact]
        public async Task OnPostUpdateDisplayStatusAsync_Good()
        {
            var actionResult = await _target.OnPostUpdateDisplayStatusAsync("Test UserName", true);

            _mockUserService.Verify(x => x.UpdateUserDisplayStatusAsync("Test UserName", true), Times.Once());
            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/USERS/MANAGE", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal($"Display status for user '@Test UserName' has been updated successfully.", details.Message);
        }

        [Fact]
        public async Task OnPostUpdateDisplayStatusAsync_Bad_InvalidModelState()
        {
            _target.ModelState.AddModelError("", "Test Error");

            var actionResult = await _target.OnPostUpdateDisplayStatusAsync("Test UserName", true);

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/USERS/MANAGE", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Error, details.Status);
            Assert.Equal($"Unable to update display status for '@Test UserName'. Reload the page and try again.", details.Message);
        }

        [Fact]
        public async Task OnPostDeleteAsync_Good()
        {
            var actionResult = await _target.OnPostDeleteAsync("Test UserName");

            _mockUserService.Verify(x => x.DeleteUserAsync("Test UserName"), Times.Once());
            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/USERS", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal($"The entity of type '{typeof(DomainUser)}' with value '@Test UserName' " +
                $"for 'UserName' deleted successfully.", details.Message);
        }

        [Fact]
        public async Task OnPostDeleteAsync_Bad_InvalidModelState()
        {
            _target.ModelState.AddModelError("", "Test Error");

            var actionResult = await _target.OnPostDeleteAsync("Test UserName");

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/USERS/MANAGE", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Error, details.Status);
            Assert.Equal($"Unable to delete entity of type '{typeof(DomainUser)}' with value '@Test UserName' " +
                    $"for 'UserName'. Reload the page and try again.", details.Message);
        }
    }
}
