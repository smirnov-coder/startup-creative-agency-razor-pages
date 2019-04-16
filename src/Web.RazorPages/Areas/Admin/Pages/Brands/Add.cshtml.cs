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
    public class AddModel : AddPageModelBase<BrandViewModel, Brand, int>
    {
        private readonly IBrandService _brandService;
        private readonly IFileService _fileService;
        private readonly BrandsAddEditCore _core;

        public AddModel(IBrandService brandService, IFileService fileService, IUserService userService) : base(userService)
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

        protected override async Task<Brand> PerformAddAsync(Brand entity)
        {
            return await _brandService.AddBrandAsync(entity);
        }
    }
}
