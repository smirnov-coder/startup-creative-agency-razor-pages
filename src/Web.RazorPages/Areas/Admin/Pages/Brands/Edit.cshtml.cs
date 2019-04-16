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
    public class EditModel : EditPageModelBase<BrandViewModel, Brand, int>
    {
        private readonly IBrandService _brandService;
        private readonly IFileService _fileService;
        private readonly BrandsAddEditCore _core;

        public EditModel(IBrandService brandService, IFileService fileService, IUserService userService) : base(userService)
        {
            _brandService = brandService;
            _fileService = fileService;
            _core = new BrandsAddEditCore(_fileService);
            RedirectUrl = "/brands";
        }

        protected override async Task<Brand> CreateEntityFromModelAsync(BrandViewModel model, DomainUser creator)
        {
            return await _core.CreateBrandFromViewModelAsync(model, creator);
        }

        protected override BrandViewModel CreateModelFromEntity(Brand entity)
        {
            var result = new BrandViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ImagePath = entity.ImagePath
            };
            return result;
        }

        protected override async Task<Brand> PerformGetSingleAsync(int entityId)
        {
            return await _brandService.GetBrandAsync(entityId);
        }

        protected override async Task PerformUpdateAsync(Brand entity)
        {
            await _brandService.UpdateBrandAsync(entity);
        }
    }
}
