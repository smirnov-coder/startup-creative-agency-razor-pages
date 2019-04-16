using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Blog
{
    public class AddModel : AddPageModelBase<BlogPostViewModel, BlogPost, int>
    {
        private readonly IBlogService _blogService;
        private readonly IFileService _fileService;
        private readonly BlogAddEditCore _core;

        public AddModel(IBlogService blogService, IFileService fileService, IUserService userService) : base(userService)
        {
            _blogService = blogService;
            _fileService = fileService;
            _core = new BlogAddEditCore(_fileService);
            RedirectUrl = "/blog";
        }

        protected override async Task<BlogPost> CreateEntityFromModelAsync(BlogPostViewModel model, DomainUser creator)
        {
            return await _core.CreateBlogPostFromViewModelAsync(model, creator);
        }

        protected override async Task<BlogPost> PerformAddAsync(BlogPost entity)
        {
            return await _blogService.AddBlogPostAsync(entity);
        }
    }
}
