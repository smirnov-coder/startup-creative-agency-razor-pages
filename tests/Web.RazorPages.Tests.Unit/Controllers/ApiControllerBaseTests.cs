using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Controllers.Api;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.Controllers
{
    public class ApiControllerBaseTests
    {
        private ApiControllerBaseUnderTest _target = new ApiControllerBaseUnderTest();

        [Fact]
        public async Task GetByIdAsync_Good()
        {
            _target.IsFound = true;

            var actionResult = await _target.GetByIdAsync(101);

            Assert.IsAssignableFrom<ActionResult<BaseEntity<int>>>(actionResult);
            var result = (actionResult as ActionResult<BaseEntity<int>>).Value;
            Assert.Equal(101, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_Bad_NotFound()
        {
            _target.IsFound = false;

            var actionResult = await _target.GetByIdAsync(101);

            Assert.IsAssignableFrom<ActionResult<BaseEntity<int>>>(actionResult);
            var result = (actionResult as ActionResult<BaseEntity<int>>).Result;
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
            string message = (result as NotFoundObjectResult).Value as string;
            Assert.Equal($"The entity of type '{typeof(BaseEntity<int>)}' with " +
                $"key value '101' for 'Id' not found.", HttpUtility.HtmlDecode(message));
        }
    }

    class ApiControllerBaseUnderTest : ApiControllerBase<BaseEntity<int>, int>
    {
        public bool IsFound { get; set; }

        protected override Task<BaseEntity<int>> PerformGetAsync(int id)
        {
            return Task.FromResult(IsFound ? new BaseEntity<int>(101) : null);
        }
    }
}
