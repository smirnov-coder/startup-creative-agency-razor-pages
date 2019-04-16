using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StartupCreativeAgency.Web.RazorPages.Infrastructure
{
    /// <summary>
    /// Вспомогательный класс пользовательского тега 'sociallink'. Генерирует иконку FontAwesome, заключённую в ссылку (html-тег 'a'),
    /// открываемую в новой вкладке браузера, на основании имён CSS-классов.
    /// </summary>
    [HtmlTargetElement("sociallink")]
    public class SocialLinkTagHelper : TagHelper
    {
        /// <summary>
        /// Имена CSS-классов иконки FontAwesome, разделённые пробелом. Например, 'fa fa-font'.
        /// </summary>
        public string Icon { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("target", "_blank");
            output.Content.AppendHtml($"<i class=\"{Icon}\"></i>");
        }
    }
}
