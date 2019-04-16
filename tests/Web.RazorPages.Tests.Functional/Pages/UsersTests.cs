using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class UsersTests
    {
        private const string PAGE_URL = "/admin/users";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public UsersTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowUserItems()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".user-item",
                        UserName = ".user-item__user-name",
                        FirstName = ".user-item__first-name",
                        LastName = ".user-item__last-name",
                        Job = ".user-item__job",
                        ReadyForDisplay = ".user-item__ready",
                        Displayed = ".user-item__displayed",
                        Photo = ".user-item__photo"
                    };
                    int expectedCount = 5;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Length);
                    Assert.Equal("@user1", items.First().QuerySelector(selectors.UserName).TextContent.Trim());
                    Assert.Equal("FirstName #1", items.First().QuerySelector(selectors.FirstName).TextContent.Trim());
                    Assert.Equal("LastName #1", items.First().QuerySelector(selectors.LastName).TextContent.Trim());
                    Assert.Equal("Job #1", items.First().QuerySelector(selectors.Job).TextContent.Trim());
                    Assert.Equal("Path #1", items.First().QuerySelector(selectors.Photo).GetAttribute("src"));
                    Assert.Equal("Yes", items.First().QuerySelector(selectors.ReadyForDisplay).TextContent.Trim());
                    Assert.Equal("Yes", items.First().QuerySelector(selectors.Displayed).TextContent.Trim());

                    Assert.Equal("@user5", items.Last().QuerySelector(selectors.UserName).TextContent.Trim());
                    Assert.Equal("FirstName #5", items.Last().QuerySelector(selectors.FirstName).TextContent.Trim());
                    Assert.Equal("LastName #5", items.Last().QuerySelector(selectors.LastName).TextContent.Trim());
                    Assert.Equal("Job #5", items.Last().QuerySelector(selectors.Job).TextContent.Trim());
                    Assert.Equal("Path #5", items.Last().QuerySelector(selectors.Photo).GetAttribute("src"));
                    Assert.Equal("Yes", items.Last().QuerySelector(selectors.ReadyForDisplay).TextContent.Trim());
                    Assert.Equal("Yes", items.Last().QuerySelector(selectors.Displayed).TextContent.Trim());
                }
            }
        }
    }
}
