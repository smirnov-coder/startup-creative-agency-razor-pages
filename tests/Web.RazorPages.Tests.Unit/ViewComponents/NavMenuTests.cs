using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Components;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.ViewComponents
{
    public class NavMenuTests
    {
        [Fact]
        public async Task InvokeAsync_Good_AdminUser()
        {
            var testMessages = new Message[]
            {
                new Message() { IsRead = false },
                new Message() { IsRead = true },
                new Message() { IsRead = false },
            };
            var testNavLinks = GetRegularUserTestNavLinkCollection()
                .Concat(GetAdminOnlyTestNavLinkCollection());
            var mockMessageService = new Mock<IMessageService>();
            mockMessageService.Setup(x => x.GetMessagesAsync()).ReturnsAsync(testMessages);
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            mockClaimsPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            var httpContext = new DefaultHttpContext { User = mockClaimsPrincipal.Object };
            var viewContext = new ViewContext { HttpContext = httpContext };
            var target = new NavMenu(mockMessageService.Object)
            {
                ViewComponentContext = new ViewComponentContext
                {
                    ViewContext = viewContext
                }
            };
            int expectedNewMessagesCount = 2;

            var result = await target.InvokeAsync() as ViewViewComponentResult;

            Assert.IsType<NavMenuViewModel>(result.ViewData.Model);
            var model = result.ViewData.Model as NavMenuViewModel;
            Assert.True(testNavLinks.SequenceEqual(model.NavLinks, new NavLinkComparer()));
            Assert.Equal(expectedNewMessagesCount, model.NewMessagesCount);
        }

        [Fact]
        public async Task InvokeAsync_Good_RegularUser()
        {
            var testNavLinks = GetRegularUserTestNavLinkCollection();
            var mockMessageService = new Mock<IMessageService>();
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            mockClaimsPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            var httpContext = new DefaultHttpContext { User = mockClaimsPrincipal.Object };
            var viewContext = new ViewContext { HttpContext = httpContext };
            var target = new NavMenu(mockMessageService.Object)
            {
                ViewComponentContext = new ViewComponentContext
                {
                    ViewContext = viewContext
                }
            };

            var result = await target.InvokeAsync() as ViewViewComponentResult;

            Assert.IsType<NavMenuViewModel>(result.ViewData.Model);
            var model = result.ViewData.Model as NavMenuViewModel;
            Assert.True(testNavLinks.SequenceEqual(model.NavLinks, new NavLinkComparer()));
        }


        private IList<NavLink> GetRegularUserTestNavLinkCollection()
        {
            return new List<NavLink>
            {
                new NavLink { Href = "/MyProfile", Text = "My Profile" },
                new NavLink { Href = "/Services", Text = "Services" },
                new NavLink { Href = "/Users", Text = "Users" },
                new NavLink { Href = "/Works", Text = "Works" },
                new NavLink { Href = "/Blog", Text = "Blog" },
                new NavLink { Href = "/Brands", Text = "Brands" },
                new NavLink { Href = "/Testimonials", Text = "Testimonials" },
            };
        }

        private IList<NavLink> GetAdminOnlyTestNavLinkCollection()
        {
            return new List<NavLink>
            {
                new NavLink { Href = "/Contacts", Text = "Contacts" },
                new NavLink { Href = "/Messages", Text = "Messages" }
            };
        }
    }

    class NavLinkComparer : IEqualityComparer<NavLink>
    {
        public bool Equals(NavLink x, NavLink y)
        {
            return x.Href == y.Href && x.Text == x.Text;
        }

        public int GetHashCode(NavLink obj)
        {
            return obj.Href.GetHashCode() ^ obj.Text.GetHashCode();
        }
    }
}
