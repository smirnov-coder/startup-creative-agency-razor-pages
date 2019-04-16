using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class MyProfileModelTests
    {
        private Mock<IUserService> _mockUserService = new Mock<IUserService>();
        private Mock<IFileService> _mockFileService = new Mock<IFileService>();
        private MyProfileModel _target;

        public MyProfileModelTests()
        {
            _target = new MyProfileModel(_mockUserService.Object, _mockFileService.Object);
            _target.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task OnGetAsync_Good()
        {
            var testUser = GetTestUser();
            _mockUserService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(testUser);

            await _target.OnGetAsync();

            Assert.NotNull(_target.PersonalInfo);
            Assert.Equal(testUser.Identity.UserName, _target.PersonalInfo.UserName);
            Assert.Equal(testUser.Profile.FirstName, _target.PersonalInfo.FirstName);
            Assert.Equal(testUser.Profile.LastName, _target.PersonalInfo.LastName);
            Assert.Equal(testUser.Profile.JobPosition, _target.PersonalInfo.JobPosition);
            Assert.Equal(testUser.Profile.PhotoFilePath, _target.PersonalInfo.PhotoFilePath);
            Assert.Equal(testUser.Profile.SocialLinks.Count, _target.SocialLinks.Count);
            Assert.Equal(testUser.Profile.SocialLinks.First().NetworkName, _target.SocialLinks.First().NetworkName);
            Assert.Equal(testUser.Profile.SocialLinks.First().Url, _target.SocialLinks.First().Url);
        }

        [Fact]
        public async Task OnPostAsync_Good()
        {
            var testUser = GetTestUser();
            var testPersonalInfo = new PersonalInfoViewModel
            {
                UserName = "New UserName",
                FirstName = "New FirstName",
                LastName = "New LastName",
                JobPosition = "New Job",
                PhotoFilePath = "New Path"
            };
            var testSocialLinks = new List<SocialLinkViewModel>
            {
                new SocialLinkViewModel { NetworkName = "Network #1", Url = "Url #1" },
                new SocialLinkViewModel { NetworkName = "Network #2", Url = "Url #2" },
                new SocialLinkViewModel { NetworkName = "Network #3", Url = "Url #3" }
            };
            _target.PersonalInfo = testPersonalInfo;
            _target.SocialLinks = testSocialLinks;
            _mockUserService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(testUser);

            var actionResult = await _target.OnPostAsync();

            _mockFileService.Verify(x => x.SaveImageAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never());
            _mockUserService.Verify(x => x.UpdateUserPersonalInfoAsync(testUser.Identity.UserName,
                testPersonalInfo.FirstName, testPersonalInfo.LastName, testPersonalInfo.JobPosition, testPersonalInfo.PhotoFilePath), Times.Once());
            _mockUserService.Verify(x => x.UpdateUserSocialLinksAsync(testUser.Identity.UserName, It.IsAny<SocialLink[]>()), Times.Once());
            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/MYPROFILE", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal("Your profile has been updated successfully.", details.Message);
        }

        private DomainUser GetTestUser()
        {
            return new DomainUser(
                new UserIdentity("Test UserName", "Test Email"),
                new UserProfile("Test FirstName", "Test LastName", "Test Job", "Test Path", new SocialLink("Test Network", "Test Url")
            ));
        }
    }
}
