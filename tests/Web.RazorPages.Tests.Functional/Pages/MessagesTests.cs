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
    public class MessagesTests
    {
        private const string PAGE_URL = "/admin/messages";
        private const string USER_NAME = "admin";
        private readonly CustomWebAppFactories _factoryCollection;

        public MessagesTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowMessageRows()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Row = ".message-row",
                        From = ".message-row__from",
                        Subject = ".message-row__subject",
                        Text = ".message-row__text"
                    };
                    int expectedCount = 3;

                    var rows = doc.QuerySelectorAll(selectors.Row);

                    Assert.Equal(expectedCount, rows.Length);
                    Assert.DoesNotContain("message-row--unread", rows.First().ClassName);
                    Assert.Equal("Name #1, Company #1 (Email #1)", rows.First().QuerySelector(selectors.From).TextContent.Trim());
                    Assert.Equal("Subject #1", rows.First().QuerySelector(selectors.Subject).TextContent.Trim());
                    Assert.Equal("Text #1", rows.First().QuerySelector(selectors.Text).TextContent.Trim());
                    Assert.Contains("message-row--unread", rows.Last().ClassName);
                    Assert.Equal("Name #3, Company #3 (Email #3)", rows.Last().QuerySelector(selectors.From).TextContent.Trim());
                    Assert.Equal("Subject #3", rows.Last().QuerySelector(selectors.Subject).TextContent.Trim());
                    Assert.Equal("Text #3", rows.Last().QuerySelector(selectors.Text).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanMarkMessagesAsUnread()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = "#messagesForm",
                        Checkbox = ".message-row__is-read",
                        Button = ".message-table__unread"
                    };
                    int expectedMessagesCount = 3,
                        expectedUnreadMessagesCount = 2;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    var requestUrl = form.QuerySelector(selectors.Button).GetAttribute("formaction");
                    var checkBoxes = form.QuerySelectorAll(selectors.Checkbox);
                    foreach (var checkBox in checkBoxes)
                    {
                        checkBox.SetAttribute("value", "false");
                    }
                    checkBoxes.First().SetAttribute("value", "true");

                    using (var response = await httpClient.PostAsync(requestUrl, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedMessagesCount, db.Messages.Count());
                                Assert.Equal(expectedUnreadMessagesCount, db.Messages.Where(x => !x.IsRead).Count());
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task CanDeleteMessages()
        {
            var factory = _factoryCollection.ForDelete;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = "#messagesForm",
                        Checkbox = ".message-row__is-read",
                        Button = ".message-table__delete"
                    };
                    int expectedCount = 1;
                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    var requestUrl = form.QuerySelector(selectors.Button).GetAttribute("formaction");
                    var checkBoxes = form.QuerySelectorAll(selectors.Checkbox);
                    foreach (var checkBox in checkBoxes)
                    {
                        checkBox.SetAttribute("value", "false");
                    }
                    checkBoxes.First().SetAttribute("value", "true");
                    checkBoxes.Last().SetAttribute("value", "true");

                    using (var response = await httpClient.PostAsync(requestUrl, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                Assert.Equal(expectedCount, db.Messages.Count());
                            }
                        }
                    }
                }
            }
        }
    }
}
