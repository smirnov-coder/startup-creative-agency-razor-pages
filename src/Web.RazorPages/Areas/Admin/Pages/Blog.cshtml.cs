using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class BlogModel : ListPageModelBase<BlogPost, int>
    {
        private readonly IBlogService _blogService;

        public BlogModel(IBlogService blogService)
        {
            _blogService = blogService;
            RedirectUrl = "/blog";
        }

        protected override async Task PerformDeleteAsync(int entityId) => await _blogService.DeleteBlogPostAsync(entityId);

        protected override async Task<IList<BlogPost>> PerformGetManyAsync() => await _blogService.GetBlogPostsAsync();
    }
}
