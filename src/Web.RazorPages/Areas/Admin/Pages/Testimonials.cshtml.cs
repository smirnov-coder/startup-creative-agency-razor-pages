using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class TestimonialsModel : ListPageModelBase<Testimonial, int>
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialsModel(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
            RedirectUrl = "/testimonials";
        }

        protected override async Task PerformDeleteAsync(int entityId) => await _testimonialService.DeleteTestimonialAsync(entityId);

        protected override async Task<IList<Testimonial>> PerformGetManyAsync() => await _testimonialService.GetTestimonialsAsync();
    }
}
