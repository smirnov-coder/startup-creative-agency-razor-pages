using System.Collections.Generic;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.ViewModels
{
    public class ClientsSectionViewModel
    {
        public IList<Brand> Brands { get; set; }
        public IList<Testimonial> Testimonials { get; set; }
    }
}
