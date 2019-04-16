using System;
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
    public class MonthTagHelperTests
    {
        [Fact]
        public async Task ProcessAsync_Good()
        {
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "testuniqueid"
            );
            var output = new TagHelperOutput("month",
                new TagHelperAttributeList()
                {
                    new TagHelperAttribute("class", "test-class")
                }, 
                (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
            );
            int monthNumber = 10;
            string expectedString = "<P CLASS=\"TEST-CLASS\">OCT</P>";
            var target = new MonthTagHelper { Number = monthNumber };

            await target.ProcessAsync(context, output);

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            output.WriteTo(writer, HtmlEncoder.Default);
            Assert.Equal(expectedString, builder.ToString().ToUpper());
        }

        [Fact]
        public async Task ProcessAsync_Bad_EmptyContent()
        {
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "testuniqueid"
            );
            var output = new TagHelperOutput("month",
                new TagHelperAttributeList(), (cache, encoder) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
            int? monthNumber = null;
            var target = new MonthTagHelper
            {
                Number = monthNumber
            };

            await target.ProcessAsync(context, output);

            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            output.WriteTo(writer, HtmlEncoder.Default);
            Assert.Equal("<P></P>", builder.ToString().ToUpper());
        }

        [Theory]
        [InlineData(0), InlineData(-1), InlineData(14)]
        public async Task ProcessAsync_Bad_ArgumentOutOfRangeException(int? number)
        {
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "testuniqueid"
            );
            var output = new TagHelperOutput("month",
                new TagHelperAttributeList(), (cache, encoder) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
            var target = new MonthTagHelper
            {
                Number = number
            };

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => target.ProcessAsync(context, output));
        }
    }
}
