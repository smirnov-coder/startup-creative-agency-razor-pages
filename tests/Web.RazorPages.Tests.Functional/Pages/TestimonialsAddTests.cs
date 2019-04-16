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
    public class TestimonialsAddTests
    {
        private const string PAGE_URL = "/admin/testimonials/add";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public TestimonialsAddTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanAddTestimonialItem()
        {
            var factory = _factoryCollection.ForAdd;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".testimonial-item-form > form",
                        Author = ".testimonial-item-form__author",
                        Company = ".testimonial-item-form__company",
                        Text = ".testimonial-item-form__text"
                    };
                    int expectedCount = 4;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Author).SetAttribute("value", "Test Author");
                    form.QuerySelector(selectors.Company).SetAttribute("value", "Test Company");
                    form.QuerySelector(selectors.Text).SetAttribute("value", "Test Text");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Testimonials.Count());
                                var testimonialItem = await db.Testimonials.LastOrDefaultAsync();
                                Assert.NotNull(testimonialItem);
                                Assert.Equal("Test Author", testimonialItem.Author);
                                Assert.Equal("Test Company", testimonialItem.Company);
                                Assert.Equal("Test Text", testimonialItem.Text);
                            }
                        }
                    }
                }
            }
        }
    }
}
