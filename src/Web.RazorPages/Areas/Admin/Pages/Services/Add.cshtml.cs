using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Services
{
    public class AddModel : AddPageModelBase<ServiceInfoViewModel, ServiceInfo, int>
    {
        private readonly IServiceInfoService _serviceInfoService;
        private readonly ServicesAddEditCore _core;

        public AddModel(IServiceInfoService serviceInfoService, IUserService userService) : base(userService)
        {
            _serviceInfoService = serviceInfoService;
            _core = new ServicesAddEditCore();
            RedirectUrl = "/services";
        }

        protected override Task<ServiceInfo> CreateEntityFromModelAsync(ServiceInfoViewModel model, DomainUser creator)
        {
            return Task.FromResult(_core.CreateServiceInfoFromViewModel(model, creator));
        }
        protected override async Task<ServiceInfo> PerformAddAsync(ServiceInfo entity)
        {
            return await _serviceInfoService.AddServiceInfoAsync(entity);
        }
    }
}
