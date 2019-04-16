using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Web.RazorPages.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Компонент представления секции "Contact"главной страницы.
    /// </summary>
    [ViewComponent]
    public class ContactSection : ViewComponent
    {
        private IContactsService _contactService;

        public ContactSection(IContactsService contactService) => _contactService = contactService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contacts = await _contactService.GetContactsAsync();
            var model = new ContactSectionViewModel
            {
                Contacts = new ContactsViewModel
                {
                    Address = contacts.FirstOrDefault(x => x.Name == "Address"),
                    Phone = contacts.FirstOrDefault(x => x.Name == "Phone"),
                    Email = contacts.FirstOrDefault(x => x.Name == "Email")
                },
                Message = new MessageViewModel()
            };
            return View(model);
        }
    }
}
