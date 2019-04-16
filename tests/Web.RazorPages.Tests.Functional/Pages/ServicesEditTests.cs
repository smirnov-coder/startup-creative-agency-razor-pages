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
    public class ServicesEditTests
    {
        private const string PAGE_URL = "/admin/services/edit/1";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public ServicesEditTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowServiceItem()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".service-item-form",
                        Id = ".service-item-form__id",
                        IconClass = ".service-item-form__icon-class",
                        Caption = ".service-item-form__caption",
                        Description = ".service-item-form__description"
                    };
                    var serviceItem = doc.QuerySelector(selectors.Item);

                    Assert.Equal("1", serviceItem.QuerySelector(selectors.Id).GetAttribute("value"));
                    Assert.Equal("Class #1", serviceItem.QuerySelector(selectors.IconClass).GetAttribute("value"));
                    Assert.Equal("Caption #1", serviceItem.QuerySelector(selectors.Caption).GetAttribute("value"));
                    Assert.Equal("Description #1", serviceItem.QuerySelector(selectors.Description).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanUpdateServiceItem()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".service-item-form > form",
                        Icon = ".service-item-form__icon-class",
                        Caption = ".service-item-form__caption",
                        Description = ".service-item-form__description"
                    };
                    int expectedCount = 3;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Icon).SetAttribute("value", "New Icon");
                    form.QuerySelector(selectors.Caption).SetAttribute("value", "New Caption");
                    form.QuerySelector(selectors.Description).SetAttribute("value", "New Description");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Services.Count());
                                var serviceItem = await db.Services.FirstOrDefaultAsync();
                                Assert.NotNull(serviceItem);
                                Assert.Equal("New Icon", serviceItem.IconClass);
                                Assert.Equal("New Caption", serviceItem.Caption);
                                Assert.Equal("New Description", serviceItem.Description);
                            }
                        }
                    }
                }
            }
        }
    }
}
