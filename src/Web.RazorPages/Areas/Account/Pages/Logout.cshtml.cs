using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Account.Pages
{
    public class LogoutModel : PageModel
    {
        private SignInManager<UserIdentity> _signInManager;

        public LogoutModel(SignInManager<UserIdentity> signInManager) => _signInManager = signInManager;

        public IActionResult OnGet() => LocalRedirect(Url.Page("/", new { area = "" }));
        
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect(returnUrl ?? Url.Page("/", new { area = "" }));
        }
    }
}
