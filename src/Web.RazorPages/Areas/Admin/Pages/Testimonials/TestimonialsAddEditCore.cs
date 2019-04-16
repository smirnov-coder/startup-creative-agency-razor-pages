using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Testimonials
{
    /// <summary>
    /// Представляет собой общую логику страниц добавления нового и обновления существующего отзыва индивидуального клиента компании.
    /// </summary>
    public class TestimonialsAddEditCore
    {
        public Testimonial CreateTestimonialFromViewModel(TestimonialViewModel model, DomainUser creator)
        {
            var testimonial = new Testimonial(model.Id, creator)
            {
                Author = model.Author,
                Company = model.Company,
                Text = model.Text
            };
            return testimonial;
        }
    }
}
