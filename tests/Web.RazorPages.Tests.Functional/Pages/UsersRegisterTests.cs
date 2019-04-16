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
    public class UsersRegisterTests
    {
        private const string PAGE_URL = "/admin/users/register";
        private const string USER_NAME = "admin";
        private readonly CustomWebAppFactories _factoryCollection;

        public UsersRegisterTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanRegisterNewUser()
        {
            var factory = _factoryCollection.ForAdd;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".user-item-form > form",
                        UserName = ".user-item-form__user-name",
                        Password = ".user-item-form__password",
                        Confirm = ".user-item-form__confirm",
                        Email = ".user-item-form__email",
                        Role = ".user-item-form__role"
                    };
                    int expectedCount = 6;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.UserName).SetAttribute("value", "testuser");
                    form.QuerySelector(selectors.Password).SetAttribute("value", "User123");
                    form.QuerySelector(selectors.Confirm).SetAttribute("value", "User123");
                    form.QuerySelector(selectors.Email).SetAttribute("value", "test@example.com");
                    var inputValues = form.GetInputValues().ToList();
                    inputValues[4] = new KeyValuePair<string, string>("NewUser.Role", "User");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(inputValues)))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                                Assert.Equal(expectedCount, userManager.Users.Count());
                                Assert.NotNull(await userManager.FindByNameAsync("testuser"));
                                Assert.Equal(expectedCount, db.DomainUsers.Count());
                                Assert.NotNull(await db.DomainUsers.FirstOrDefaultAsync(x => x.Identity.UserName == "testuser"));
                            }
                        }
                    }
                }
            }
        }
    }
}
