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
    public class EditModel : EditPageModelBase<BlogPostViewModel, BlogPost, int>
    {
        private readonly IBlogService _blogService;
        private readonly IFileService _fileService;
        private readonly BlogAddEditCore _core;

        public EditModel(IBlogService blogService, IFileService fileService, IUserService userService) : base(userService)
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

        protected override BlogPostViewModel CreateModelFromEntity(BlogPost entity)
        {
            var result = new BlogPostViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Category = entity.Category,
                Content = entity.Content,
                ImagePath = entity.ImagePath
            };
            return result;
        }

        protected override async Task<BlogPost> PerformGetSingleAsync(int entityId)
        {
            return await _blogService.GetBlogPostAsync(entityId);
        }

        protected override async Task PerformUpdateAsync(BlogPost entity)
        {
            await _blogService.UpdateBlogPostAsync(entity);
        }
    }
}
