using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Testimonials
{
    public class EditModel : EditPageModelBase<TestimonialViewModel, Testimonial, int>
    {
        private readonly ITestimonialService _testimonialService;
        private readonly TestimonialsAddEditCore _core;

        public EditModel(ITestimonialService testimonialService, IUserService userService) : base(userService)
        {
            _testimonialService = testimonialService;
            _core = new TestimonialsAddEditCore();
            RedirectUrl = "/testimonials";
        }

        protected override Task<Testimonial> CreateEntityFromModelAsync(TestimonialViewModel model, DomainUser creator)
        {
            return Task.FromResult(_core.CreateTestimonialFromViewModel(model, creator));
        }

        protected override TestimonialViewModel CreateModelFromEntity(Testimonial entity)
        {
            var result = new TestimonialViewModel
            {
                Id = entity.Id,
                Author = entity.Author,
                Company = entity.Company,
                Text = entity.Text
            };
            return result;
        }

        protected override async Task<Testimonial> PerformGetSingleAsync(int entityId)
        {
            return await _testimonialService.GetTestimonialAsync(entityId);
        }

        protected override async Task PerformUpdateAsync(Testimonial entity)
        {
            await _testimonialService.UpdateTestimonialAsync(entity);
        }
    }
}
