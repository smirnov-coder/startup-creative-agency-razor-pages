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
    public class BrandsTests
    {
        private const string PAGE_URL = "/admin/brands";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public BrandsTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowBrandItems()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".brand-item",
                        Id = ".brand-item__id",
                        Name = ".brand-item__name",
                        Image = ".brand-item__img",
                    };
                    int expectedCount = 5;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Length);
                    Assert.Equal("1", items.First().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Name #1", items.First().QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal("Path #1", items.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("5", items.Last().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Name #5", items.Last().QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal("Path #5", items.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                }
            }
        }

        [Fact]
        public async Task CanDeleteBrandItem()
        {
            var factory = _factoryCollection.ForDelete;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".brand-item",
                        Id = ".brand-item__id"
                    };
                    int expectedCount = 4;
                    var brandItemToDelete = doc.QuerySelectorAll(selectors.Item).First();
                    int brandItemId = int.Parse(brandItemToDelete.QuerySelector(selectors.Id).TextContent.Trim());
                    var form = brandItemToDelete.QuerySelector("form") as IHtmlFormElement;

                    using (var response = await httpClient.PostAsync(form.Action, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Brands.Count());
                                Assert.DoesNotContain(db.Brands, x => x.Id == brandItemId);
                            }
                        }
                    }
                }
            }
        }
    }
}
