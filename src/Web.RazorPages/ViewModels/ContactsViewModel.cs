using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.ViewModels
{
    public class ContactsViewModel
    {
        public ContactInfo Address { get; set; }
        public ContactInfo Phone { get; set; }
        public ContactInfo Email { get; set; }
    }
}
