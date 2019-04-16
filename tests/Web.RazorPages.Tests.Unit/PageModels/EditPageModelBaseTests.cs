using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class EditPageModelBaseTests
    {
        private Mock<IUserService> _mockUserService = new Mock<IUserService>();
        private EditPageModelBaseUnderTest _target;

        public EditPageModelBaseTests()
        {
            _mockUserService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(new DomainUser(new UserIdentity()));
            _target = new EditPageModelBaseUnderTest(_mockUserService.Object);
        }

        [Fact]
        public async Task OnGetAsync_Good()
        {
            _target.IsFound = true;

            var actionResult = await _target.OnGetAsync(101);

            Assert.IsAssignableFrom<PageResult>(actionResult);
            var result = actionResult as PageResult;
            Assert.NotNull(_target.Model);
            Assert.Equal(101, _target.Model.Id);
        }

        [Fact]
        public async Task OnGetAsync_Bad_NotFound()
        {
            var actionResult = await _target.OnGetAsync(101);

            Assert.IsAssignableFrom<NotFoundResult>(actionResult);
            //var result = (actionResult as NotFoundResult).Value;
            //Assert.IsType<int>(result);
            //Assert.Equal(101, (int)result);
            Assert.Null(_target.Model);
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
                $"'{nameof(BaseEntity<int>.Id)}' updated successfully.", details.Message);
        }
    }

    class EditPageModelBaseUnderTest : EditPageModelBase<TestViewModel, BaseEntity<int>, int>
    {
        public EditPageModelBaseUnderTest(IUserService userService)
            : base(userService)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            RedirectUrl = "/test";
        }

        protected override Task<BaseEntity<int>> CreateEntityFromModelAsync(TestViewModel model, DomainUser creator)
        {
            return Task.FromResult(new BaseEntity<int>(model.Id));
        }

        protected override TestViewModel CreateModelFromEntity(BaseEntity<int> entity)
        {
            return new TestViewModel(entity.Id);
        }

        public bool IsFound { get; set; }

        protected override Task<BaseEntity<int>> PerformGetSingleAsync(int entityId)
        {
            var result = IsFound ? new BaseEntity<int>(entityId) : null;
            return Task.FromResult(result);
        }

        protected override Task PerformUpdateAsync(BaseEntity<int> entity)
        {
            return Task.CompletedTask;
        }
    }
}
