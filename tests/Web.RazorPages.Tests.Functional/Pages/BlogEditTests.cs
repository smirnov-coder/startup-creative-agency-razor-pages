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
    public class BlogEditTests
    {
        private const string PAGE_URL = "/admin/blog/edit/1";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public BlogEditTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowBlogPostItem()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".blog-post-item-form",
                        Id = ".blog-post-item-form__id",
                        Title = ".blog-post-item-form__title",
                        Category = ".blog-post-item-form__category",
                        Content = ".blog-post-item-form__content",
                        Image = ".blog-post-item-form__img"
                    };
                    var blogPostItem = doc.QuerySelector(selectors.Item);

                    Assert.Equal("1", blogPostItem.QuerySelector(selectors.Id).GetAttribute("value"));
                    Assert.Equal("Title #1", blogPostItem.QuerySelector(selectors.Title).GetAttribute("value"));
                    Assert.Equal("Category #1", blogPostItem.QuerySelector(selectors.Category).GetAttribute("value"));
                    Assert.Equal("Content #1", blogPostItem.QuerySelector(selectors.Content).TextContent.Trim());
                    Assert.Equal("Path #1", blogPostItem.QuerySelector(selectors.Image).GetAttribute("src"));
                }
            }
        }

        [Fact]
        public async Task CanUpdateBlogPostItem()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".blog-post-item-form > form",
                        Title = ".blog-post-item-form__title",
                        Category = ".blog-post-item-form__category",
                        Content = ".blog-post-item-form__content",
                    };
                    int expectedCount = 4;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Title).SetAttribute("value", "New Title");
                    form.QuerySelector(selectors.Category).SetAttribute("value", "New Category");
                    form.QuerySelector(selectors.Content).SetAttribute("value", "New Content");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.BlogPosts.Count());
                                var blogPostItem = await db.BlogPosts.FirstOrDefaultAsync();
                                Assert.NotNull(blogPostItem);
                                Assert.Equal("New Title", blogPostItem.Title);
                                Assert.Equal("New Category", blogPostItem.Category);
                                Assert.Equal("New Content", blogPostItem.Content);
                            }
                        }
                    }                        
                }
            }
        }
    }
}
