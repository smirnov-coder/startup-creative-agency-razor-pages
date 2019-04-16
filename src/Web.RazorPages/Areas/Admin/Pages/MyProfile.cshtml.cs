using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class MyProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public MyProfileModel(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        [BindProperty]
        public PersonalInfoViewModel PersonalInfo { get; set; }

        [BindProperty]
        public IList<SocialLinkViewModel> SocialLinks { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userService.GetUserAsync(User?.Identity?.Name);
            PersonalInfo = new PersonalInfoViewModel
            {
                UserName = user.Identity.UserName,
                FirstName = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                JobPosition = user.Profile.JobPosition,
                PhotoFilePath = user.Profile.PhotoFilePath
            };

            SocialLinks = user.Profile.SocialLinks.Select(x => new SocialLinkViewModel
            {
                NetworkName = x.NetworkName,
                Url = x.Url
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userService.GetUserAsync(User?.Identity?.Name);
            string photoFilePath = PersonalInfo.PhotoFilePath;
            if (PersonalInfo.Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    await PersonalInfo.Image.CopyToAsync(stream);
                    string extension = Path.GetExtension(PersonalInfo.Image.FileName);
                    string fileName = _fileService.GenerateUniqueFileName(extension, "userphoto-");
                    photoFilePath = await _fileService.SaveImageAsync(fileName, stream);
                }
            }
            await _userService.UpdateUserPersonalInfoAsync(user.Identity.UserName, PersonalInfo.FirstName,
                PersonalInfo.LastName, PersonalInfo.JobPosition, photoFilePath);

            var socialLinks = SocialLinks.Select(x => new SocialLink(x.NetworkName, x.Url));
            await _userService.UpdateUserSocialLinksAsync(user.Identity.UserName, socialLinks.ToArray());

            this.SetDetails(OperationStatus.Success, "Your profile has been updated successfully.");
            return RedirectToPage("/MyProfile");
        }
    }
}