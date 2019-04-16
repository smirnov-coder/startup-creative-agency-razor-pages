using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class AuthTests
    {
        private readonly CustomWebAppFactories _factoryCollection;

        public AuthTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Theory]
        [MemberData(nameof(PagesTestData.PublicPages), MemberType = typeof(PagesTestData))]
        public async Task CanGetAnonymousAccessToPublicPages(string pageUrl, string title)
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                using (var response = await httpClient.GetAsync(pageUrl))
                {
                    Assert.True(response.IsSuccessStatusCode);
                    HtmlParser parser = new HtmlParser();
                    using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync()))
                    {
                        Assert.Equal($"Startup {title}", doc.QuerySelector("title").TextContent.Trim());
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(PrivatePages))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public async Task CanRedirectAnonymousUsersToLoginPage(string pageUrl, string title)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
            using (var httpClient = _factoryCollection.ForRead.CreateClient(options))
            {
                using (var response = await httpClient.GetAsync(pageUrl))
                {
                    Assert.True(!response.IsSuccessStatusCode);
                    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                    Assert.Equal("/ACCOUNT/LOGIN", response.Headers.Location.LocalPath.ToUpper());
                }
            }
        }

        [Theory]
        [MemberData(nameof(PagesTestData.AdminOnlyPages), MemberType = typeof(PagesTestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public async Task CanRedirectUnauthorizedUsersToAccessDeniedPage(string pageUrl, string title)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync("user1", options))
            {
                using (var response = await httpClient.GetAsync(pageUrl))
                {
                    Assert.True(!response.IsSuccessStatusCode);
                    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                    Assert.Equal("/ACCOUNT/ACCESSDENIED", response.Headers.Location.LocalPath.ToUpper());
                }
            }
        }

        public static IEnumerable<object[]> PrivatePages =>
            PagesTestData.PrivatePages.Concat(PagesTestData.AdminOnlyPages);
    }
}
