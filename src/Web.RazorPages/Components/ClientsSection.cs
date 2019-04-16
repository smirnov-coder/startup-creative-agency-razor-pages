using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Web.RazorPages.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Компонент представления секции "Clients" главной страницы.
    /// </summary>
    [ViewComponent]
    public class ClientsSection : ViewComponent
    {
        private IBrandService _brandService;
        private ITestimonialService _testimonialService;

        public ClientsSection(IBrandService brandService, ITestimonialService testimonialService)
        {
            _brandService = brandService;
            _testimonialService = testimonialService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ClientsSectionViewModel
            {
                Brands = await _brandService.GetBrandsAsync(),
                Testimonials = await _testimonialService.GetTestimonialsAsync()
            };
            return View(model);
        }
    }
}
