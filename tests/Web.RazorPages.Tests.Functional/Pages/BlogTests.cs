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
    public class BlogTests
    {
        private const string PAGE_URL = "/admin/blog";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public BlogTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowBlogPostItems()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".blog-post-item",
                        Id = ".blog-post-item__id",
                        Image = ".blog-post-item__img",
                        Title = ".blog-post-item__title",
                        Category = ".blog-post-item__category",
                        Content = ".blog-post-item__content"
                    };
                    int expectedCount = 4;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Length);
                    Assert.Equal("4", items.First().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Path #4", items.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Title #4", items.First().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("Category #4", items.First().QuerySelector(selectors.Category).TextContent.Trim());
                    Assert.Equal("Content #4", items.First().QuerySelector(selectors.Content).TextContent.Trim());
                    Assert.Equal("1", items.Last().QuerySelector(selectors.Id).TextContent.Trim());
                    Assert.Equal("Path #1", items.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Title #1", items.Last().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("Category #1", items.Last().QuerySelector(selectors.Category).TextContent.Trim());
                    Assert.Equal("Content #1", items.Last().QuerySelector(selectors.Content).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanDeleteBlogPostItem()
        {
            var factory = _factoryCollection.ForDelete;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".blog-post-item",
                        Id = ".blog-post-item__id"
                    };
                    int expectedCount = 3;
                    var blogPostItemToDelete = doc.QuerySelectorAll(selectors.Item).First();
                    int blogPostItemId = int.Parse(blogPostItemToDelete.QuerySelector(selectors.Id).TextContent.Trim());
                    var form = blogPostItemToDelete.QuerySelector("form") as IHtmlFormElement;

                    using (var response = await httpClient.PostAsync(form.Action, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.BlogPosts.Count());
                                Assert.DoesNotContain(db.BlogPosts, x => x.Id == blogPostItemId);
                            }
                        }
                    }
                }
            }
        }
    }
}
