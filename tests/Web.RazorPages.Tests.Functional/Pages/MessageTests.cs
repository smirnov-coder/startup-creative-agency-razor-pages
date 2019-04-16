using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class MessageTests
    {
        private const string PAGE_URL = "/admin/message/1";
        private const string USER_NAME = "admin";
        private readonly CustomWebAppFactories _factoryCollection;

        public MessageTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowMessage()
        {
            using (var httpClient = await _factoryCollection.ForRead.CreateClientWithAuthCookiesAsync(USER_NAME))
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Message = ".message",
                        //Id = ".service-item-form__id",
                        Name = ".message__name",
                        Company = ".message__company",
                        Email = ".message__email",
                        Subject = ".message__subject",
                        Text = ".message__text"
                    };
                    var message = doc.QuerySelector(selectors.Message);

                    Assert.Equal("Name #1", message.QuerySelector(selectors.Name).TextContent.Trim());
                    Assert.Equal("Company #1", message.QuerySelector(selectors.Company).TextContent.Trim());
                    Assert.Equal("Email #1", message.QuerySelector(selectors.Email).TextContent.Trim());
                    Assert.Equal("Subject #1", message.QuerySelector(selectors.Subject).TextContent.Trim());
                    Assert.Equal("Text #1", message.QuerySelector(selectors.Text).TextContent.Trim());
                }
            }
        }
    }
}
