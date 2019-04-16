using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Attributes;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class PersonalInfoViewModel
    {
        [BindNever]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Job Position")]
        [StringLength(100)]
        public string JobPosition { get; set; }

        [DataType(DataType.ImageUrl)]
        public string PhotoFilePath { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Photo")]
        [ImageFileExtensions]
        public IFormFile Image { get; set; }
    }
}