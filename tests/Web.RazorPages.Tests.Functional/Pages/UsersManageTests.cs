using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class UsersManageTests
    {
        private const string PAGE_URL = "/admin/users/manage/user2";
        private const string USER_NAME = "admin";
        private readonly CustomWebAppFactories _factoryCollection;

        public UsersManageTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowUserItem()
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

                    var item = doc.QuerySelector(selectors.Item);

                    Assert.Equal("@user2", item.QuerySelector(selectors.UserName).TextContent.Trim());
                    Assert.Equal("FirstName #2", item.QuerySelector(selectors.FirstName).TextContent.Trim());
                    Assert.Equal("LastName #2", item.QuerySelector(selectors.LastName).TextContent.Trim());
                    Assert.Equal("Job #2", item.QuerySelector(selectors.Job).TextContent.Trim());
                    Assert.Equal("Path #2", item.QuerySelector(selectors.Photo).GetAttribute("src"));
                    Assert.Equal("Yes", item.QuerySelector(selectors.ReadyForDisplay).TextContent.Trim());
                    Assert.Equal("Yes", item.QuerySelector(selectors.Displayed).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanDeleteUserItem()
        {
            var factory = _factoryCollection.ForDelete;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".user-item",
                        UserName = ".user-item__user-name",
                        Delete = ".user-item__delete"
                    };
                    int expectedCount = 4;
                    var item = doc.QuerySelector(selectors.Item);
                    string userName = item.QuerySelector(selectors.UserName).TextContent.Trim().Substring(1);
                    var form = item.QuerySelector("form") as IHtmlFormElement;
                    string requestUrl = form.QuerySelector(selectors.Delete).GetAttribute("formaction");

                    using (var response = await httpClient.PostAsync(requestUrl, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>())
                                {
                                    Assert.Equal(expectedCount, userManager.Users.Count());
                                    Assert.Equal(expectedCount, db.DomainUsers.Count());
                                    Assert.DoesNotContain(userManager.Users, x => x.UserName == userName);
                                }
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task CanUpdateUserDisplayStatus()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".user-item",
                        UserName = ".user-item__user-name",
                        Update = ".user-item__update",
                        DisplayStatus = ".user-item__is-displayed"
                    };
                    var item = doc.QuerySelector(selectors.Item);
                    string userName = item.QuerySelector(selectors.UserName).TextContent.Trim().Substring(1);
                    var statusCheckbox = item.QuerySelector(selectors.DisplayStatus);
                    bool displayStatus = statusCheckbox.HasAttribute("checked");
                    statusCheckbox.RemoveAttribute("checked");
                    statusCheckbox.SetAttribute("value", "false");
                    var form = item.QuerySelector("form") as IHtmlFormElement;
                    string requestUrl = form.QuerySelector(selectors.Update).GetAttribute("formaction");

                    using (var response = await httpClient.PostAsync(requestUrl, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>())
                                {
                                    var modifiedUser = await db.DomainUsers.FirstOrDefaultAsync(x => x.Identity.UserName == userName);
                                    Assert.NotNull(modifiedUser);
                                    Assert.Equal(!displayStatus, modifiedUser.Profile.DisplayAsTeamMember);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
