using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class LoginTests
    {
        private const string PAGE_URL = "/account/login";
        private readonly CustomWebAppFactories _factoryCollection;

        public LoginTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanSignIn()
        {
            var factory = _factoryCollection.ForAdd;//
            using (var httpClient = factory.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(PAGE_URL)))
                {
                    var selectors = new
                    {
                        Form = ".login-form > form",
                        UserName = ".login-form__user-name",
                        Password = ".login-form__password",
                        RememberMe = ".login-form__remember-me"
                    };

                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.UserName).SetAttribute("value", "user");
                    form.QuerySelector(selectors.Password).SetAttribute("value", "User123");

                    using (var response = await httpClient.PostAsync(PAGE_URL, new FormUrlEncodedContent(form.GetInputValues())))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                    }
                }
            }
        }
    }
}
