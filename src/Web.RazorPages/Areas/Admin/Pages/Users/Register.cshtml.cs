using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Users
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            UserRoles = new SelectList(roleManager.Roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name));
        }

        public SelectList UserRoles { get; set; }

        [BindProperty]
        public RegisterUserViewModel NewUser { get; set; }

        public void OnGet() => NewUser = new RegisterUserViewModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserAsync(NewUser.UserName);
                if (user == null)
                {
                    var creator = await _userService.GetUserAsync(User?.Identity?.Name);
                    await _userService.CreateUserAsync(NewUser.UserName, NewUser.Password, NewUser.Email, NewUser.Role, creator);
                    this.SetDetails(OperationStatus.Success, $"User '@{NewUser.UserName}' has been registered successfully.");
                    return RedirectToPage("/users");
                }
                else
                {
                    ModelState.AddModelError($"{nameof(NewUser)}.{nameof(NewUser.UserName)}", 
                        $"User '@{NewUser.UserName}' already exists.");
                }
            }
            return Page();
        }
    }
}
