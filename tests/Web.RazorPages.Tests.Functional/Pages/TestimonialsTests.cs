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
    public class TestimonialsTests
    {
        private const string PAGE_URL = "/admin/testimonials";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public TestimonialsTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowTestimonialItems()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".testimonial-item",
                        Id = ".testimonial-item__id",
                        Author = ".testimonial-item__author",
                        Company = ".testimonial-item__company",
                        Text = ".testimonial-item__text"
                    };
                    int expectedCount = 3;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Length);
                    Assert.Equal("1", items.First().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Author #1", items.First().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Company #1", items.First().QuerySelector(selectors.Company).TextContent.Trim());
                    Assert.Equal("Text #1", items.First().QuerySelector(selectors.Text).TextContent.Trim());
                    Assert.Equal("3", items.Last().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Author #3", items.Last().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Company #3", items.Last().QuerySelector(selectors.Company).TextContent.Trim());
                    Assert.Equal("Text #3", items.Last().QuerySelector(selectors.Text).TextContent.Trim());
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
                        Item = ".testimonial-item",
                        Id = ".testimonial-item__id"
                    };
                    int expectedCount = 2;
                    var testimonialItemToDelete = doc.QuerySelectorAll(selectors.Item).First();
                    int testimonialItemId = int.Parse(testimonialItemToDelete.QuerySelector(selectors.Id).TextContent.Trim());
                    var form = testimonialItemToDelete.QuerySelector("form") as IHtmlFormElement;

                    using (var response = await httpClient.PostAsync(form.Action, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Testimonials.Count());
                                Assert.DoesNotContain(db.Testimonials, x => x.Id == testimonialItemId);
                            }
                        }
                    }
                }
            }
        }
    }
}
