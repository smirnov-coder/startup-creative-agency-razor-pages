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
    public class BrandsAddTests
    {
        private const string PAGE_URL = "/admin/brands/add";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public BrandsAddTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanAddBrandItem()
        {
            var factory = _factoryCollection.ForAdd;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".brand-item-form > form",
                        Name = ".brand-item-form__name",
                        Upload = ".brand-item-form__upload"
                    };
                    int expectedCount = 6;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Name).SetAttribute("value", "Test Name");
                    string fileInputName = form.QuerySelector(selectors.Upload).GetAttribute("name");

                    using (var requestContent = TestHelper.CreateTestMultipartFormDataContent(form.GetInputValues(), fileInputName, "test-brand-item.jpg"))
                    {
                        using (var response = await httpClient.PostAsync(PAGE_URL, requestContent))
                        {
                            Assert.True(response.IsSuccessStatusCode);
                            // Проверяем данные изолированного хоста.
                            using (var scope = factory.Server.Host.Services.CreateScope())
                            {
                                using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                                {
                                    Assert.Equal(expectedCount, db.Brands.Count());
                                    var brandItem = await db.Brands.LastOrDefaultAsync();
                                    Assert.NotNull(brandItem);
                                    Assert.Equal("Test Name", brandItem.Name);
                                    string fileName = Path.GetFileNameWithoutExtension(brandItem.ImagePath);
                                    Assert.StartsWith("brand-", fileName);
                                    Assert.Equal($"~/images/{fileName}.jpg", brandItem.ImagePath);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
