using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Areas.Account.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Account.Pages
{
    public class LoginModel : PageModel
    {
        private SignInManager<UserIdentity> _signInManager;

        public LoginModel(SignInManager<UserIdentity> signInManager) => _signInManager = signInManager;

        [BindProperty]
        public LoginViewModel LoginInfo { get; set; }

        public void OnGet() => LoginInfo = new LoginViewModel();

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(LoginInfo.UserName);
                if (user != null)
                {
                    if (await _signInManager.UserManager.CheckPasswordAsync(user, LoginInfo.Password))
                    {
                        var result = await _signInManager.PasswordSignInAsync(LoginInfo.UserName, LoginInfo.Password,
                            LoginInfo.RememberMe, false);
                        if (result.Succeeded)
                            return LocalRedirect(returnUrl ?? Url.Page("myprofile", new { area = "admin" }));
                        else
                            ModelState.AddModelError("", "Failed to sign in. Try again later.");
                    }
                    else
                    {
                        ModelState.AddModelError($"{nameof(LoginInfo)}.{nameof(LoginInfo.Password)}", "Invalid password.");
                    }
                }
                else
                {
                    ModelState.AddModelError($"{nameof(LoginInfo)}.{nameof(LoginInfo.UserName)}", "User not found.");
                }
            }
            return Page();
        }
    }
}
