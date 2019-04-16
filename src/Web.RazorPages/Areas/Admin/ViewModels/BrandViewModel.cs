using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Attributes;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class BrandViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Current Image")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        [AtLeastOneOfTwo(nameof(ImagePath), ErrorMessage = "The Image must be provided.")]
        [ImageFileExtensions]
        public IFormFile Image { get; set; }
    }
}
