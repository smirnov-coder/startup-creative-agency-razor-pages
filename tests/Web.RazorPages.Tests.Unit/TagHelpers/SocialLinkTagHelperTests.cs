using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.TagHelpers
{
    public class SocialLinkTagHelperTests
    {
        [Theory]
        [InlineData(null), InlineData(""), InlineData(" "), InlineData("test-icon")]
        public async Task ProcessAsync_Good(string iconClass)
        {
            var context = new TagHelperContext(
               new TagHelperAttributeList(),
               new Dictionary<object, object>(),
               "testuniqueid"
            );
            var output = new TagHelperOutput("sociallink",
                new TagHelperAttributeList
                {
                    new TagHelperAttribute("href", "test-href"),
                    new TagHelperAttribute("class", "test-class")
                },
                (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
            );
            string expectedString = 
                $"<A HREF=\"TEST-HREF\" CLASS=\"TEST-CLASS\" TARGET=\"_BLANK\"><I CLASS=\"{iconClass?.ToUpper()}\"></I></A>";
            var target = new SocialLinkTagHelper { Icon = iconClass };

            await target.ProcessAsync(context, output);

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            output.WriteTo(writer, HtmlEncoder.Default);
            Assert.Equal(expectedString, builder.ToString().ToUpper());
        }
    }
}
