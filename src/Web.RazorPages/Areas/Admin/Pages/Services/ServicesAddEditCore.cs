using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Services
{
    /// <summary>
    /// Представляет собой общую логику страниц добавления нового и обновления существующего описания услуги компании.
    /// </summary>
    public class ServicesAddEditCore
    {
        public ServiceInfo CreateServiceInfoFromViewModel(ServiceInfoViewModel model, DomainUser creator)
        {
            var serviceInfo = new ServiceInfo(model.Id, creator)
            {
                IconClass = model.IconClass,
                Caption = model.Caption,
                Description = model.Description,
            };
            return serviceInfo;
        }
    }
}
