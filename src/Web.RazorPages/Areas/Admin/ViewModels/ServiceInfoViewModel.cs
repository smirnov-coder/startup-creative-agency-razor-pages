using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class ServiceInfoViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "FontAwesome Icon Class")]
        public string IconClass { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Caption")]
        public string Caption { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
