using System;
using System.Collections.Generic;
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
    public class BrandsEditTests
    {
        private const string PAGE_URL = "/admin/brands/edit/1";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public BrandsEditTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowBrandItem()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".brand-item-form",
                        Id = ".brand-item-form__id",
                        Name = ".brand-item-form__name",
                        Image = ".brand-item-form__img"
                    };
                    var brandItem = doc.QuerySelector(selectors.Item);

                    Assert.Equal("1", brandItem.QuerySelector(selectors.Id).GetAttribute("value"));
                    Assert.Equal("Name #1", brandItem.QuerySelector(selectors.Name).GetAttribute("value"));
                    Assert.Equal("Path #1", brandItem.QuerySelector(selectors.Image).GetAttribute("src"));
                }
            }
        }

        [Fact]
        public async Task CanUpdateBrandItem()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".brand-item-form > form",
                        Name = ".brand-item-form__name"
                    };
                    int expectedCount = 5;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Name).SetAttribute("value", "New Name");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Brands.Count());
                                var brandItem = await db.Brands.FirstOrDefaultAsync();
                                Assert.NotNull(brandItem);
                                Assert.Equal("New Name", brandItem.Name);
                            }
                        }
                    }
                }
            }
        }
    }
}
