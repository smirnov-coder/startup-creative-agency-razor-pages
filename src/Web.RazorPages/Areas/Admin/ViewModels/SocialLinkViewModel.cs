using System.ComponentModel.DataAnnotations;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class SocialLinkViewModel
    {
        [Required]
        [StringLength(50)]
        public string NetworkName { get; set; }

        [StringLength(100)]
        [DataType(DataType.Url)]
        public string Url { get; set; }
    }
}
