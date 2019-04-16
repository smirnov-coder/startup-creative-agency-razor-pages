using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class TestimonialViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Author { get; set; }

        [Required]
        [StringLength(50)]
        public string Company { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }
    }
}
