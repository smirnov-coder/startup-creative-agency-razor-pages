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
    public class TestimonialsEditTests
    {
        private const string PAGE_URL = "/admin/testimonials/edit/1";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public TestimonialsEditTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowTestimonialItem()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".testimonial-item-form",
                        Id = ".testimonial-item-form__id",
                        Author = ".testimonial-item-form__author",
                        Company = ".testimonial-item-form__company",
                        Text = ".testimonial-item-form__text"
                    };
                    var testimonialItem = doc.QuerySelector(selectors.Item);

                    Assert.Equal("1", testimonialItem.QuerySelector(selectors.Id).GetAttribute("value"));
                    Assert.Equal("Author #1", testimonialItem.QuerySelector(selectors.Author).GetAttribute("value"));
                    Assert.Equal("Company #1", testimonialItem.QuerySelector(selectors.Company).GetAttribute("value"));
                    Assert.Equal("Text #1", testimonialItem.QuerySelector(selectors.Text).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanUpdateTestimonialItem()
        {
            var factory = _factoryCollection.ForUpdate;
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
                    int expectedCount = 3;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Author).SetAttribute("value", "New Author");
                    form.QuerySelector(selectors.Company).SetAttribute("value", "New Company");
                    form.QuerySelector(selectors.Text).SetAttribute("value", "New Text");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Services.Count());
                                var testimonialItem = await db.Testimonials.FirstOrDefaultAsync();
                                Assert.NotNull(testimonialItem);
                                Assert.Equal("New Author", testimonialItem.Author);
                                Assert.Equal("New Company", testimonialItem.Company);
                                Assert.Equal("New Text", testimonialItem.Text);
                            }
                        }
                    }
                }
            }
        }
    }
}
