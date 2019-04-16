using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Services
{
    public class EditModel : EditPageModelBase<ServiceInfoViewModel, ServiceInfo, int>
    {
        private readonly IServiceInfoService _serviceInfoService;
        private readonly ServicesAddEditCore _core;

        public EditModel(IServiceInfoService serviceInfoService, IUserService userService) : base(userService)
        {
            _serviceInfoService = serviceInfoService;
            _core = new ServicesAddEditCore();
            RedirectUrl = "/services";
        }

        protected override async Task<ServiceInfo> PerformGetSingleAsync(int entityId)
        {
            return await _serviceInfoService.GetServiceInfoAsync(entityId);
        }

        protected override Task<ServiceInfo> CreateEntityFromModelAsync(ServiceInfoViewModel model, DomainUser creator)
        {
            return Task.FromResult(_core.CreateServiceInfoFromViewModel(model, creator));
        }

        protected override async Task PerformUpdateAsync(ServiceInfo entity)
        {
            await _serviceInfoService.UpdateServiceInfoAsync(entity);
        }

        protected override ServiceInfoViewModel CreateModelFromEntity(ServiceInfo entity)
        {
            var result = new ServiceInfoViewModel
            {
                Id = entity.Id,
                IconClass = entity.IconClass,
                Caption = entity.Caption,
                Description = entity.Description
            };
            return result;
        }
    }
}
