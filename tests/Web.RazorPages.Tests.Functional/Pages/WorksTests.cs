using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class WorksTests
    {
        private const string PAGE_URL = "/admin/works";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public WorksTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowWorkExampleItems()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".work-example-item",
                        Id = ".work-example-item__id",
                        Name = ".work-example-item__name",
                        Category = ".work-example-item__category",
                        Description = ".work-example-item__description",
                        Image = ".work-example-item__img"
                    };
                    int expectedCount = 9;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Length);
                    Assert.Equal("1", items.First().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Name #1", items.First().QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal("Category #1", items.First().QuerySelector(selectors.Category).TextContent.Trim());
                    Assert.Equal("Description #1", items.First().QuerySelector(selectors.Description).TextContent.Trim());
                    Assert.Equal("Path #1", items.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("9", items.Last().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Name #9", items.Last().QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal("Category #2", items.Last().QuerySelector(selectors.Category).TextContent.Trim());
                    Assert.Equal("Description #9", items.Last().QuerySelector(selectors.Description).TextContent.Trim());
                    Assert.Equal("Path #9", items.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                }
            }
        }

        [Fact]
        public async Task CanDeleteWorkExampleItem()
        {
            var factory = _factoryCollection.ForDelete;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".work-example-item",
                        Id = ".work-example-item__id"
                    };
                    int expectedCount = 8;
                    var workExampleItemToDelete = doc.QuerySelectorAll(selectors.Item).First();
                    int workExampleItemId = int.Parse(workExampleItemToDelete.QuerySelector(selectors.Id).TextContent.Trim());
                    var form = workExampleItemToDelete.QuerySelector("form") as IHtmlFormElement;

                    using (var response = await httpClient.PostAsync(form.Action, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Works.Count());
                                Assert.DoesNotContain(db.Works, x => x.Id == workExampleItemId);
                            }
                        }
                    }
                }
            }
        }
    }
}
