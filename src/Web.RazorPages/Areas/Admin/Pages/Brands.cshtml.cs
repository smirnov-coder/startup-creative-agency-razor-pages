using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class BrandsModel : ListPageModelBase<Brand, int>
    {
        private readonly IBrandService _brandService;

        public BrandsModel(IBrandService brandService)
        {
            _brandService = brandService;
            RedirectUrl = "/brands";
        }

        protected override async Task PerformDeleteAsync(int entityId) => await _brandService.DeleteBrandAsync(entityId);

        protected override async Task<IList<Brand>> PerformGetManyAsync() => await _brandService.GetBrandsAsync();
    }
}
