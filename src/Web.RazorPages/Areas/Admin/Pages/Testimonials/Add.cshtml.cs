using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Testimonials
{
    public class AddModel : AddPageModelBase<TestimonialViewModel, Testimonial, int>
    {
        private readonly ITestimonialService _testimonialService;
        private readonly TestimonialsAddEditCore _core;

        public AddModel(ITestimonialService testimonialService, IUserService userService) : base(userService)
        {
            _testimonialService = testimonialService;
            _core = new TestimonialsAddEditCore();
            RedirectUrl = "/testimonials";
        }

        protected override Task<Testimonial> CreateEntityFromModelAsync(TestimonialViewModel model, DomainUser creator)
        {
            return Task.FromResult(_core.CreateTestimonialFromViewModel(model, creator));
        }

        protected override async Task<Testimonial> PerformAddAsync(Testimonial entity)
        {
            return await _testimonialService.AddTestimonialAsync(entity);
        }
    }
}
