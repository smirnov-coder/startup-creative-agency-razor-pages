using System.ComponentModel.DataAnnotations;

namespace StartupCreativeAgency.Web.RazorPages.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
        public string Email { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        [Required]
        [StringLength(5000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }
}
