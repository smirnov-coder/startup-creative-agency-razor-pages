using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Компонент представления секции "Blog" главной страницы.
    /// </summary>
    [ViewComponent]
    public class BlogSection : ViewComponent
    {
        private IBlogService _blogService;

        /// <summary>
        /// Количество отображаемых блог постов на главной странице. Значение по умолчанию 2.
        /// </summary>
        public int BlogPostsDisplayedCount { get; set; } = 2;

        public BlogSection(IBlogService blogService) => _blogService = blogService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _blogService.GetBlogPostsAsync(take: BlogPostsDisplayedCount);
            return View(model);
        }
    }
}
