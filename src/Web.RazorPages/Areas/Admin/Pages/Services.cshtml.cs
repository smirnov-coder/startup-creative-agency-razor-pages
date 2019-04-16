using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class ServicesModel : ListPageModelBase<ServiceInfo, int>
    {
        private readonly IServiceInfoService _serviceInfoService;

        public ServicesModel(IServiceInfoService serviceInfoService)
        {
            _serviceInfoService = serviceInfoService;
            RedirectUrl = "/services";
        }

        protected override async Task PerformDeleteAsync(int entityId) => await _serviceInfoService.DeleteServiceInfoAsync(entityId);

        protected override async Task<IList<ServiceInfo>> PerformGetManyAsync() => await _serviceInfoService.GetServiceInfosAsync();
    }
}
