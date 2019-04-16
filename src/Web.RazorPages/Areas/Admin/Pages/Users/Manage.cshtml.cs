using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Users
{
    public class ManageModel : PageModel
    {
        private const string USERS_URL = "/users";
        private const string MANAGE_URL = "/users/manage";
        private IUserService _userService;

        public ManageModel(IUserService userService) => _userService = userService;

        public DomainUser DomainUser { get; set; }

        public async Task<IActionResult> OnGetAsync([FromRoute]string userName)
        {
            if ((DomainUser = await _userService.GetUserAsync(userName)) == null)
                return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateDisplayStatusAsync(string userName, bool isDisplayed)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateUserDisplayStatusAsync(userName, isDisplayed);
                this.SetDetails(OperationStatus.Success, $"Display status for user '@{userName}' " +
                    $"has been updated successfully.");
            }
            else
            {
                this.SetDetails(OperationStatus.Error, $"Unable to update display status for '@{userName}'. " +
                    $"Reload the page and try again.");
            }
            return RedirectToPage(MANAGE_URL, new { userName });
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userName)
        {
            if (ModelState.IsValid)
            {
                await _userService.DeleteUserAsync(userName);
                this.SetDetails(OperationStatus.Success, $"The entity of type '{typeof(DomainUser)}' with value '@{userName}' " +
                    $"for '{nameof(Domain.Entities.DomainUser.Identity.UserName)}' deleted successfully.");
                return RedirectToPage(USERS_URL);
            }
            else
            {
                this.SetDetails(OperationStatus.Error, $"Unable to delete entity of type '{typeof(DomainUser)}' with value '@{userName}' " +
                    $"for '{nameof(Domain.Entities.DomainUser.Identity.UserName)}'. Reload the page and try again.");
                return RedirectToPage(MANAGE_URL, new { userName });
            }
        }
    }
}
