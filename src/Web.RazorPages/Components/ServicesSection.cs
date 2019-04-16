using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Компонент представления секции "Services" главной страницы.
    /// </summary>
    [ViewComponent]
    public class ServicesSection : ViewComponent
    {
        private IServiceInfoService _serviceInfoService;

        public ServicesSection(IServiceInfoService serviceInfoService) => _serviceInfoService = serviceInfoService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _serviceInfoService.GetServiceInfosAsync();
            return View(model);
        }
    }
}
