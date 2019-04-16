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
    public class WorksEditTests
    {
        private const string PAGE_URL = "/admin/works/edit/1";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public WorksEditTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowWorkExampleItem()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".work-example-item-form",
                        Id = ".work-example-item-form__id",
                        Name = ".work-example-item-form__name",
                        Category = ".work-example-item-form__category",
                        Description = ".work-example-item-form__description",
                        Image = ".work-example-item-form__img"
                    };
                    var workExampleItem = doc.QuerySelector(selectors.Item);

                    Assert.Equal("1", workExampleItem.QuerySelector(selectors.Id).GetAttribute("value"));
                    Assert.Equal("Name #1", workExampleItem.QuerySelector(selectors.Name).GetAttribute("value"));
                    Assert.Equal("Category #1", workExampleItem.QuerySelector(selectors.Category).GetAttribute("value"));
                    Assert.Equal("Description #1", workExampleItem.QuerySelector(selectors.Description).TextContent.Trim());
                    Assert.Equal("Path #1", workExampleItem.QuerySelector(selectors.Image).GetAttribute("src"));
                }
            }
        }

        [Fact]
        public async Task CanUpdateWorkExampleItem()
        {
            var factory = _factoryCollection.ForUpdate;
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
                        Description = ".work-example-item-form__description"
                    };
                    int expectedCount = 9;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Name).SetAttribute("value", "New Name");
                    form.QuerySelector(selectors.Category).SetAttribute("value", "New Category");
                    form.QuerySelector(selectors.Description).SetAttribute("value", "New Description");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Works.Count());
                                var workExampleItem = await db.Works.FirstOrDefaultAsync();
                                Assert.NotNull(workExampleItem);
                                Assert.Equal("New Name", workExampleItem.Name);
                                Assert.Equal("New Category", workExampleItem.Category);
                                Assert.Equal("New Description", workExampleItem.Description);
                            }
                        }
                    }
                }
            }
        }
    }
}
