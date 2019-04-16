using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Attributes;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class BlogPostViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; }

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
