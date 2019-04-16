using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class AccessibilityTests
    {
        private const string USER_NAME = "admin";
        private readonly CustomWebAppFactories _factoryCollection;

        public AccessibilityTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Theory]
        [MemberData(nameof(Pages))]
        public async Task CanGetPage(string pageUrl, string title)
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
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

        public static IEnumerable<object[]> Pages =>
            PagesTestData.PublicPages.Concat(PagesTestData.PrivatePages.Concat(PagesTestData.AdminOnlyPages));
    }
}
