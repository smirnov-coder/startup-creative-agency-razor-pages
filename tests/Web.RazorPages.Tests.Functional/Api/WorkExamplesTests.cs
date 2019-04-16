using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StartupCreativeAgency.Domain.Entities;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Api
{
    [Collection("Factories")]
    public class WorkExamplesTests
    {
        private readonly CustomWebAppFactories _factoryCollection;

        public WorkExamplesTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanGetWorkExample()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                using (var response = await httpClient.GetAsync("api/workexamples/1"))
                {
                    Assert.True(response.IsSuccessStatusCode);
                    Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var workExample = JsonConvert.DeserializeObject<WorkExample>(resultJson);
                    Assert.NotNull(workExample);
                    Assert.Equal(1, workExample.Id);
                    Assert.Equal("Name #1", workExample.Name);
                    Assert.Equal("Path #1", workExample.ImagePath);
                    Assert.Equal("Category #1", workExample.Category);
                    Assert.Equal("Description #1", workExample.Description);
                }
            }
        }
    }
}
