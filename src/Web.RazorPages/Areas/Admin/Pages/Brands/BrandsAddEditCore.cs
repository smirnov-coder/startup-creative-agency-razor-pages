using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Brands
{
    /// <summary>
    /// Представляет собой общую логику страниц добавления нового и обновления существующего бренда.
    /// </summary>
    public class BrandsAddEditCore
    {
        private readonly IFileService _fileService;

        public BrandsAddEditCore(IFileService fileService) => _fileService = fileService;

        public async Task<Brand> CreateBrandFromViewModelAsync(BrandViewModel model, DomainUser creator)
        {
            var brand = new Brand(model.Id, creator)
            {
                Name = model.Name,
                ImagePath = model.ImagePath
            };
            if (model.Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    await model.Image.CopyToAsync(stream);
                    string extension = Path.GetExtension(model.Image.FileName);
                    string fileName = _fileService.GenerateUniqueFileName(extension, "brand-");
                    brand.ImagePath = await _fileService.SaveImageAsync(fileName, stream);
                }
            }
            return brand;
        }
    }
}
