using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Users;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class RegisterModelTests
    {
        private Mock<IUserService> _mockUserService = new Mock<IUserService>();
        private Mock<RoleManager<IdentityRole>> _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
        private RegisterModel _target;

        public RegisterModelTests()
        {
            _target = new RegisterModel(_mockUserService.Object, _mockRoleManager.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()),
            };
        }

        [Fact]
        public void Ctor_Good()
        {
            var testRoles = new List<IdentityRole>
            {
                new IdentityRole("Role #1"),
                new IdentityRole("Role #2"),
                new IdentityRole("Role #3")
            };
            _mockRoleManager.Setup(x => x.Roles).Returns(testRoles.AsQueryable());

            _target = new RegisterModel(_mockUserService.Object, _mockRoleManager.Object);

            Assert.Equal(3, _target.UserRoles.Count());
            Assert.Equal("Role #1", _target.UserRoles.First().Value);
            Assert.Equal("Role #3", _target.UserRoles.Last().Value);
        }

        [Fact]
        public async Task OnPostAsync_Good()
        {
            var testUserData = GetTestUserData();
            var testCreator = new DomainUser(new UserIdentity());
            _mockUserService.Setup(x => x.GetUserAsync(testUserData.UserName)).ReturnsAsync(default(DomainUser));
            _mockUserService.Setup(x => x.GetUserAsync(null)).ReturnsAsync(testCreator);
            _target.NewUser = testUserData;

            var actionResult = await _target.OnPostAsync();

            _mockUserService.Verify(x => x.CreateUserAsync(testUserData.UserName, testUserData.Password,
                testUserData.Email, testUserData.Role, testCreator), Times.Once());
            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/USERS", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal("User '@Test UserName' has been registered successfully.", details.Message);
        }

        private RegisterUserViewModel GetTestUserData() =>
            new RegisterUserViewModel
            {
                UserName = "Test UserName",
                Password = "Test Password",
                ConfirmPassword = "Test Password",
                Email = "Test Email",
                Role = "Test Role"
            };
    }
}
