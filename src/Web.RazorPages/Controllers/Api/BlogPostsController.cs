using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Controllers.Api
{
    /// <summary>
    /// Контроллер для работы с блогом.
    /// </summary>
    public class BlogPostsController : ApiControllerBase<BlogPost, int>
    {
        private readonly IBlogService _blogService;

        public BlogPostsController(IBlogService blogService)
        {
            _blogService = blogService;
            _blogService.FetchThreshold = 10;
        }

        protected override async Task<BlogPost> PerformGetAsync(int id) => await _blogService.GetBlogPostAsync(id);

        protected override void PrepareEntityForReturn(BlogPost entity) => entity.ImagePath = Url.Content(entity.ImagePath);

        //
        // GET api/BlogPosts/RenderedPreviews?skip=0&take=5
        //
        // Возвращает часть коллекции блог постов в виде HTML-разметки.
        [HttpGet("RenderedPreviews")]
        public async Task<IActionResult> GetRenderedBlogPostPreviewsAsync(int skip, int take)
        {
            var model = await _blogService.GetBlogPostsAsync(skip, take);

            return new PartialViewResult
            {
                ViewName = "_BlogPartial",
                ViewData = new ViewDataDictionary(MetadataProvider, ModelState)
                {
                    Model = model
                }
            };
        }
    }
}
