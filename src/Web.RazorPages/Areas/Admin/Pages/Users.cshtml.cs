using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class UsersModel : PageModel
    {
        private IUserService _userService;

        public UsersModel(IUserService userService) => _userService = userService;

        public IList<DomainUser> Users { get; set; }

        public async Task OnGetAsync() => Users = await _userService.GetUsersAsync();
    }
}
