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
    public class ServicesAddTests
    {
        private const string PAGE_URL = "/admin/services/add";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public ServicesAddTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }
        
        [Fact]
        public async Task CanAddServiceItem()
        {
            var factory = _factoryCollection.ForAdd;
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
                    int expectedCount = 4;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Icon).SetAttribute("value", "Test Icon");
                    form.QuerySelector(selectors.Caption).SetAttribute("value", "Test Caption");
                    form.QuerySelector(selectors.Description).SetAttribute("value", "Test Description");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Services.Count());
                                var serviceItem = await db.Services.LastOrDefaultAsync();
                                Assert.NotNull(serviceItem);
                                Assert.Equal("Test Icon", serviceItem.IconClass);
                                Assert.Equal("Test Caption", serviceItem.Caption);
                                Assert.Equal("Test Description", serviceItem.Description);
                            }
                        }
                    }
                }
            }
        }
    }
}
