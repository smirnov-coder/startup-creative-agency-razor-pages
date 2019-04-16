using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Копмонент представления футера главной страницы.
    /// </summary>
    [ViewComponent]
    public class FooterSection : ViewComponent
    {
        private IContactsService _contactsService;

        public FooterSection(IContactsService contactsService) => _contactsService = contactsService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _contactsService.GetSocialLinksAsync();
            return View(model);
        }
    }
}
