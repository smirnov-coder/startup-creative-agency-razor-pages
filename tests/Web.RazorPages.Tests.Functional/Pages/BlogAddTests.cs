using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class BlogAddTests
    {
        private const string PAGE_URL = "/admin/blog/add";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public BlogAddTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanAddBlogPostItem()
        {
            var factory = _factoryCollection.ForAdd;
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
                        Upload = ".blog-post-item-form__upload"
                    };
                    int expectedCount = 5;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Title).SetAttribute("value", "Test Title");
                    form.QuerySelector(selectors.Category).SetAttribute("value", "Test Category");
                    form.QuerySelector(selectors.Content).SetAttribute("value", "Test Content");
                    string fileInputName = form.QuerySelector(selectors.Upload).GetAttribute("name");

                    using (var requestContent = TestHelper.CreateTestMultipartFormDataContent(form.GetInputValues(), fileInputName, "test-blog-item.jpg"))
                    {
                        using (var response = await httpClient.PostAsync(PAGE_URL, requestContent))
                        {
                            Assert.True(response.IsSuccessStatusCode);
                            // Проверяем данные изолированного хоста.
                            using (var scope = factory.Server.Host.Services.CreateScope())
                            {
                                using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                                {
                                    Assert.Equal(expectedCount, db.BlogPosts.Count());
                                    var blogPostItem = await db.BlogPosts.LastOrDefaultAsync();
                                    Assert.NotNull(blogPostItem);
                                    Assert.Equal("Test Title", blogPostItem.Title);
                                    Assert.Equal("Test Category", blogPostItem.Category);
                                    Assert.Equal("Test Content", blogPostItem.Content);
                                    string fileName = Path.GetFileNameWithoutExtension(blogPostItem.ImagePath);
                                    Assert.StartsWith("blogpost-", fileName);
                                    Assert.Equal($"~/images/{fileName}.jpg", blogPostItem.ImagePath);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
