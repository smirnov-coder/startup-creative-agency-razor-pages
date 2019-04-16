using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class AddPageModelBaseTests
    {
        private Mock<IUserService> _mockUserService = new Mock<IUserService>();
        private AddPageModelBaseUnderTest _target;

        public AddPageModelBaseTests()
        {
            _mockUserService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(new DomainUser(new UserIdentity()));
            _target = new AddPageModelBaseUnderTest(_mockUserService.Object);
        }

        [Fact]
        public async Task OnPostAsync_Good()
        {
            _target.Model = new TestViewModel(101);

            var actionResult = await _target.OnPostAsync();

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/TEST", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal($"The entity of type '{typeof(BaseEntity<int>)}' with key value '101' for " +
                $"'{nameof(BaseEntity<int>.Id)}' saved successfully.", details.Message);
        }
    }

    class AddPageModelBaseUnderTest : AddPageModelBase<TestViewModel, BaseEntity<int>, int>
    {
        public AddPageModelBaseUnderTest(IUserService userService)
            : base(userService)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            RedirectUrl = "/test";
        }

        protected override Task<BaseEntity<int>> CreateEntityFromModelAsync(TestViewModel model, DomainUser creator)
        {
            return Task.FromResult(new BaseEntity<int>(model.Id));
        }


        protected override Task<BaseEntity<int>> PerformAddAsync(BaseEntity<int> entity)
        {
            return Task.FromResult(entity);
        }
    }
}
