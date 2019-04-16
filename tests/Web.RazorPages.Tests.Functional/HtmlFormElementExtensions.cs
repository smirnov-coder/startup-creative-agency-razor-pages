using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Html.Dom;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional
{
    public static class HtmlFormElementExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> GetInputValues(this IHtmlFormElement form)
        {
            return form.Elements
                .Select(element => new KeyValuePair<string, string>(element.GetAttribute("name"), element.GetAttribute("value")))
                .Where(x => !string.IsNullOrEmpty(x.Key));
        }
    }
}
