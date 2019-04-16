using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Компонент представления секции "About" главной страницы.
    /// </summary>
    [ViewComponent]
    public class AboutSection : ViewComponent
    {
        private IUserService _userService;

        public AboutSection(IUserService userService) => _userService = userService;
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _userService.GetDisplayedTeamMembersAsync();
            return View(model);
        }
    }
}
