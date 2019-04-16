using System.Globalization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StartupCreativeAgency.Web.RazorPages.Infrastructure
{
    /// <summary>
    /// Вспомогательный класс пользовательского тега 'month'. Преобразует целочисленный номер месяца в строку,
    /// состоящую из аббревиатуры (трёх первых букв) названия месяца в нижнем регистре на английском языке, 
    /// заключённую в html-тег 'p'.
    /// </summary>
    [HtmlTargetElement("month")]
    public class MonthTagHelper : TagHelper
    {
        /// <summary>
        /// Номер месяца.
        /// </summary>
        public int? Number { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "p";
            output.TagMode = TagMode.StartTagAndEndTag;
            string month = string.Empty;
            if (Number.HasValue)
            {
                month = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(Number.Value);
            }
            output.Content.SetContent(month.ToLower());
        }
    }
}
