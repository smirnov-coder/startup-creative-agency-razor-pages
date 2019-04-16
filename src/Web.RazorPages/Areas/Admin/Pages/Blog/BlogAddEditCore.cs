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
    /// <summary>
    /// Представляет собой общую логику страниц добавления нового и обновления существующего блог поста.
    /// </summary>
    public class BlogAddEditCore
    {
        private IFileService _fileService;

        public BlogAddEditCore(IFileService fileService) => _fileService = fileService;

        public async Task<BlogPost> CreateBlogPostFromViewModelAsync(BlogPostViewModel model, DomainUser creator)
        {
            var blogPost = new BlogPost(model.Id, creator)
            {
                Title = model.Title,
                Category = model.Category,
                Content = model.Content,
                ImagePath = model.ImagePath
            };
            if (model.Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    await model.Image.CopyToAsync(stream);
                    string extension = Path.GetExtension(model.Image.FileName);
                    string fileName = _fileService.GenerateUniqueFileName(extension, "blogpost-");
                    blogPost.ImagePath = await _fileService.SaveImageAsync(fileName, stream);
                }
            }
            return blogPost;
        }
    }
}
