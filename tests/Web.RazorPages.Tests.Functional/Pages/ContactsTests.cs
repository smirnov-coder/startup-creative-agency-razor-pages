using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Domain.Abstractions.Services;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class ContactsTests
    {
        private const string PAGE_URL = "/admin/contacts";
        private const string USER_NAME = "admin";
        private readonly CustomWebAppFactories _factoryCollection;

        public ContactsTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowContacts()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Item = ".contact-item",
                        Line = ".contact-item__line",
                        Name = ".contact-item__name",
                        Label = ".contact-item__label",
                        Input = ".contact-item__text-input"
                    };
                    int expectedItemsCount = 4,
                        expectedLinesCount = 4;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedItemsCount, items.Length);
                    Assert.Equal("Address", items.First().QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal(expectedLinesCount, items.First().QuerySelectorAll(selectors.Line).Length);
                    Assert.Equal("Caption", items.First().QuerySelectorAll(selectors.Line).First()
                        .QuerySelector(selectors.Label).TextContent.Trim());
                    Assert.Equal("Caption #1", items.First().QuerySelectorAll(selectors.Line).First()
                        .QuerySelector(selectors.Input).GetAttribute("value"));
                    Assert.Equal("Address #3", items.First().QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Label).TextContent.Trim());
                    Assert.Equal("Value #3", items.First().QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Input).GetAttribute("value"));

                    Assert.Equal("Social Links", items.Last().QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal(expectedLinesCount, items.Last().QuerySelectorAll(selectors.Line).Length);
                    Assert.Equal("Facebook", items.Last().QuerySelectorAll(selectors.Line).First()
                        .QuerySelector(selectors.Label).TextContent.Trim());
                    Assert.Equal("Link #1", items.Last().QuerySelectorAll(selectors.Line).First()
                        .QuerySelector(selectors.Input).GetAttribute("value"));
                    Assert.Equal("Linkedin", items.Last().QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Label).TextContent.Trim());
                    Assert.Equal("Link #4", items.Last().QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Input).GetAttribute("value"));
                }
            }
        }

        [Fact]
        public async Task CanUpdateContacts()
        {
            var factory = _factoryCollection.ForUpdate;
            using (var httpClient = await factory.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".contact-list > form",
                        Item = ".contact-item",
                        Line = ".contact-item__line",
                        Name = ".contact-item__name",
                        Label = ".contact-item__label",
                        Input = ".contact-item__text-input"
                    };

                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelectorAll(selectors.Item)[0].QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Input).SetAttribute("value", "New Address");
                    form.QuerySelectorAll(selectors.Item)[1].QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Input).SetAttribute("value", "New Phone");
                    form.QuerySelectorAll(selectors.Item)[2].QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Input).SetAttribute("value", "New Email");
                    form.QuerySelectorAll(selectors.Item)[3].QuerySelectorAll(selectors.Line).Last()
                        .QuerySelector(selectors.Input).SetAttribute("value", "New Link");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        // Проверяем данные изолированного хоста.
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            var contactsService = scope.ServiceProvider.GetRequiredService<IContactsService>();
                            var contacts = await contactsService.GetContactsAsync();
                            Assert.Equal("New Address", contacts.FirstOrDefault(x => x.Name == "Address").Values.Last());
                            Assert.Equal("New Phone", contacts.FirstOrDefault(x => x.Name == "Phone").Values.Last());
                            Assert.Equal("New Email", contacts.FirstOrDefault(x => x.Name == "Email").Values.Last());
                            var socialLinks = await contactsService.GetSocialLinksAsync();
                            Assert.Equal("New Link", socialLinks["Linkedin"]);
                        }
                    }
                }
            }
        }
    }
}
