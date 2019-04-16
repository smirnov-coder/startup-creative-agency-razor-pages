using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class ContactsModel : PageModel
    {
        private readonly IContactsService _contactsService;

        public ContactsModel(IContactsService contactsService) => _contactsService = contactsService;

        [BindProperty]
        public IList<ContactInfoViewModel> Contacts { get; set; }

        [BindProperty]
        public IList<SocialLinkViewModel> SocialLinks { get; set; }

        public async Task OnGetAsync()
        {
            Contacts = (await _contactsService.GetContactsAsync()).Select(contact => new ContactInfoViewModel
            {
                Caption = contact.Caption,
                Name = contact.Name,
                Values = contact.Values.Select(x => new ContactValue { Value = x }).ToList()
            }).ToList();

            SocialLinks = (await _contactsService.GetSocialLinksAsync()).Select(socialLink => new SocialLinkViewModel
            {
                NetworkName = socialLink.Key,
                Url = socialLink.Value
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var contacts = Contacts.Select(contact => new ContactInfo(contact.Name)
            {
                Caption = contact.Caption,
                Values = contact.Values.Select(x => x.Value).ToList()
            });
            await _contactsService.SaveContactsAsync(contacts);

            var socialLinks = SocialLinks.ToDictionary(socialLink => socialLink.NetworkName, socialLink => socialLink.Url);
            await _contactsService.SaveSocialLinksAsync(socialLinks);

            this.SetDetails(OperationStatus.Success, "Company contacts saved successfully.");
            return RedirectToPage("/contacts");
        }
    }
}
