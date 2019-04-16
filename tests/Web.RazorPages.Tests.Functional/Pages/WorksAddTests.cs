using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class WorksAddTests
    {
        private const string PAGE_URL = "/admin/works/add";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public WorksAddTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanAddWorkExampleItem()
        {
            var factory = _factoryCollection.ForAdd;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".work-example-item-form > form",
                        Name = ".work-example-item-form__name",
                        Category = ".work-example-item-form__category",
                        Description = ".work-example-item-form__description",
                        Upload = ".work-example-item-form__upload"
                    };
                    int expectedCount = 10;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Name).SetAttribute("value", "Test Name");
                    form.QuerySelector(selectors.Category).SetAttribute("value", "Test Category");
                    form.QuerySelector(selectors.Description).SetAttribute("value", "Test Description");
                    string fileInputName = form.QuerySelector(selectors.Upload).GetAttribute("name");

                    using (var requestContent = TestHelper.CreateTestMultipartFormDataContent(form.GetInputValues(), fileInputName, "test-work-example.jpg"))
                    {
                        using (var response = await httpClient.PostAsync(PAGE_URL, requestContent))
                        {
                            Assert.True(response.IsSuccessStatusCode);
                            // Проверяем данные изолированного хоста.
                            using (var scope = factory.Server.Host.Services.CreateScope())
                            {
                                using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                                {
                                    Assert.Equal(expectedCount, db.Works.Count());
                                    var workExampleItem = await db.Works.LastOrDefaultAsync();
                                    Assert.NotNull(workExampleItem);
                                    Assert.Equal("Test Name", workExampleItem.Name);
                                    Assert.Equal("Test Category", workExampleItem.Category);
                                    Assert.Equal("Test Description", workExampleItem.Description);
                                    string fileName = Path.GetFileNameWithoutExtension(workExampleItem.ImagePath);
                                    Assert.StartsWith("workexample-", fileName);
                                    Assert.Equal($"~/images/{fileName}.jpg", workExampleItem.ImagePath);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
