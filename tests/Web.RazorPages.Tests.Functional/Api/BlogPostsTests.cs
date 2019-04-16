using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StartupCreativeAgency.Domain.Entities;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Api
{
    [Collection("Factories")]
    public class BlogPostsTests
    {
        private readonly CustomWebAppFactories _factoryCollection;

        public BlogPostsTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanGetBlogPost()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                using (var response = await httpClient.GetAsync("api/blogposts/1"))
                {
                    Assert.True(response.IsSuccessStatusCode);
                    Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var blogPost = JsonConvert.DeserializeObject<BlogPost>(resultJson);
                    Assert.NotNull(blogPost);
                    Assert.Equal(1, blogPost.Id);
                    Assert.Equal("Title #1", blogPost.Title);
                    Assert.Equal("Path #1", blogPost.ImagePath);
                    Assert.Equal("Category #1", blogPost.Category);
                    Assert.Equal("Content #1", blogPost.Content);
                }
            }
        }
    }
}
