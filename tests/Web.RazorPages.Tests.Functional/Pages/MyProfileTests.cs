using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public class MyProfileTests
    {
        private const string PAGE_URL = "/admin/myprofile";
        private const string USER_NAME = "user1";
        private readonly CustomWebAppFactories _factoryCollection;

        public MyProfileTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowUserProfileInfo()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        PersonalInfo = ".personal-info",
                        Photo = ".personal-info__photo",
                        UserName = ".personal-info__user-name",
                        FirstName = ".personal-info__first-name",
                        LastName = ".personal-info__last-name",
                        Job = ".personal-info__job",
                        SocialLinkLine = ".contact-item__line",
                        Network = ".contact-item__label",
                        Url = ".contact-item__text-input"
                    };
                    int expectedCount = 4;

                    var personalInfo = doc.QuerySelector(selectors.PersonalInfo);
                    Assert.Equal("Path #1", personalInfo.QuerySelector(selectors.Photo).GetAttribute("src"));
                    Assert.Equal("user1", personalInfo.QuerySelector(selectors.UserName).TextContent.Trim());
                    Assert.Equal("FirstName #1", personalInfo.QuerySelector(selectors.FirstName).GetAttribute("value"));
                    Assert.Equal("LastName #1", personalInfo.QuerySelector(selectors.LastName).GetAttribute("value"));
                    Assert.Equal("Job #1", personalInfo.QuerySelector(selectors.Job).GetAttribute("value"));
                    var socialLinks = doc.QuerySelectorAll(selectors.SocialLinkLine);
                    Assert.Equal(expectedCount, socialLinks.Length);
                    Assert.Equal("Facebook", socialLinks.First().QuerySelector(selectors.Network).TextContent.Trim());
                    Assert.Equal("Link #1", socialLinks.First().QuerySelector(selectors.Url).GetAttribute("value"));
                    Assert.Equal("Linkedin", socialLinks.Last().QuerySelector(selectors.Network).TextContent.Trim());
                    Assert.Equal("Link #4", socialLinks.Last().QuerySelector(selectors.Url).GetAttribute("value"));
                }
            }
        }

        [Fact]
        public async Task CanUpdateUserProfileInfo()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = "#userInfoForm",
                        FirstName = ".personal-info__first-name",
                        LastName = ".personal-info__last-name",
                        Job = ".personal-info__job",
                        Upload = ".personal-info__upload",
                        SocialLinkUrl = ".contact-item__text-input"
                    };

                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.FirstName).SetAttribute("value", "New FirstName");
                    form.QuerySelector(selectors.LastName).SetAttribute("value", "New LastName");
                    form.QuerySelector(selectors.Job).SetAttribute("value", "New Job");
                    form.QuerySelectorAll(selectors.SocialLinkUrl)[0].SetAttribute("value", "New Link #1");
                    form.QuerySelectorAll(selectors.SocialLinkUrl)[1].SetAttribute("value", "New Link #2");
                    form.QuerySelectorAll(selectors.SocialLinkUrl)[2].SetAttribute("value", "New Link #3");
                    form.QuerySelectorAll(selectors.SocialLinkUrl)[3].SetAttribute("value", "New Link #4");
                    string fileInputName = form.QuerySelector(selectors.Upload).GetAttribute("name");

                    using (var requestContent = TestHelper.CreateTestMultipartFormDataContent(form.GetInputValues(), fileInputName, "test-user-photo.jpg"))
                    {
                        using (var response = await httpClient.PostAsync(PAGE_URL, requestContent))
                        {
                            Assert.True(response.IsSuccessStatusCode);
                            // Проверяем данные изолированного хоста.
                            using (var scope = factory.Server.Host.Services.CreateScope())
                            {
                                using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                                {
                                    var result = await db.DomainUsers.FirstOrDefaultAsync(x => x.Identity.UserName == "user1");
                                    Assert.NotNull(result);
                                    Assert.Equal("New FirstName", result.Profile.FirstName);
                                    Assert.Equal("New LastName", result.Profile.LastName);
                                    Assert.Equal("New Job", result.Profile.JobPosition);
                                    string fileName = Path.GetFileNameWithoutExtension(result.Profile.PhotoFilePath);
                                    Assert.StartsWith("userphoto-", fileName);
                                    Assert.Equal($"~/images/{fileName}.jpg", result.Profile.PhotoFilePath);
                                    Assert.Equal("New Link #1", result.Profile.SocialLinks[0].Url);
                                    Assert.Equal("New Link #2", result.Profile.SocialLinks[1].Url);
                                    Assert.Equal("New Link #3", result.Profile.SocialLinks[2].Url);
                                    Assert.Equal("New Link #4", result.Profile.SocialLinks[3].Url);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
