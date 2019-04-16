using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class ListPageModelBaseTests
    {
        private ListPageModelBaseUnderTest _target = new ListPageModelBaseUnderTest();

        [Fact]
        public async Task OnGetAsync_Good()
        {
            await _target.OnGetAsync();

            Assert.Equal(3, _target.Model.Count);
            Assert.Equal(101, _target.Model.First().Id);
            Assert.Equal(103, _target.Model.Last().Id);
        }

        [Fact]
        public async Task OnPostDeleteAsync_Good()
        {
            var actionResult = await _target.OnPostDeleteAsync(101);

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/TEST", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal($"The entity of type '{typeof(BaseEntity<int>)}' with key value '101' for " +
                $"'{nameof(BaseEntity<int>.Id)}' deleted successfully.", details.Message);
        }

        [Fact]
        public async Task OnPostDeleteAsync_Bad()
        {
            _target.ModelState.AddModelError("", "Test Error");

            var actionResult = await _target.OnPostDeleteAsync(101);

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/TEST", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Error, details.Status);
            Assert.Equal($"Unable to delete entity of type '{typeof(BaseEntity<int>)}' with key value '101' for " +
                    $"'{nameof(BaseEntity<int>.Id)}'. Reload the page and try again.", details.Message);
        }
    }

    class ListPageModelBaseUnderTest : ListPageModelBase<BaseEntity<int>, int>
    {
        public ListPageModelBaseUnderTest()
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            RedirectUrl = "/test";
        }

        protected override Task PerformDeleteAsync(int entityId)
        {
            return Task.CompletedTask;
        }

        protected override Task<IList<BaseEntity<int>>> PerformGetManyAsync()
        {
            IList<BaseEntity<int>> result = new List<BaseEntity<int>>
            {
                new BaseEntity<int>(101),
                new BaseEntity<int>(102),
                new BaseEntity<int>(103)
            };
            return Task.FromResult(result);
        }
    }
}
