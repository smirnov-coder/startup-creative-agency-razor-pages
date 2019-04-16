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
    public class ServicesTests
    {
        private const string PAGE_URL = "/admin/services";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public ServicesTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }
        
        [Fact]
        public async Task CanShowServiceItems()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".service-item",
                        Id = ".service-item__id",
                        Icon = ".service-item__icon > i",
                        Caption = ".service-item__caption",
                        Description = ".service-item__description"
                    };
                    int expectedCount = 3;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Length);
                    Assert.Equal("1", items.First().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Class #1", items.First().QuerySelector(selectors.Icon).ClassName);
                    Assert.Equal("Caption #1", items.First().QuerySelector(selectors.Caption).TextContent.Trim());
                    Assert.Equal("Description #1", items.First().QuerySelector(selectors.Description).TextContent.Trim());
                    Assert.Equal("3", items.Last().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Class #3", items.Last().QuerySelector(selectors.Icon).ClassName);
                    Assert.Equal("Caption #3", items.Last().QuerySelector(selectors.Caption).TextContent.Trim());
                    Assert.Equal("Description #3", items.Last().QuerySelector(selectors.Description).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanDeleteServiceItem()
        {
            var factory = _factoryCollection.ForDelete;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".service-item",
                        Id = ".service-item__id"
                    };
                    int expectedCount = 2;
                    var serviceItemToDelete = doc.QuerySelectorAll(selectors.Item).First();
                    int serviceItemId = int.Parse(serviceItemToDelete.QuerySelector(selectors.Id).TextContent.Trim());
                    var form = serviceItemToDelete.QuerySelector("form") as IHtmlFormElement;

                    using (var response = await httpClient.PostAsync(form.Action, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Services.Count());
                                Assert.DoesNotContain(db.Services, x => x.Id == serviceItemId);
                            }
                        }
                    }
                }
            }
        }
    }
}
