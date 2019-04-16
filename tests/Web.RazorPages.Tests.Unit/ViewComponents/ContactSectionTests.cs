using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Components;
using StartupCreativeAgency.Web.RazorPages.ViewModels;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.ViewComponents
{
    public class ContactSectionTests
    {
        [Fact]
        public async Task InvokeAsync_Good()
        {
            var testContacts = new List<ContactInfo>
            {
                new ContactInfo("Address")
                {
                    Caption = "Address",
                    Values = new string[] { "Value #1", "Value #2", "Value #3" }
                },
                new ContactInfo("Phone")
                {
                    Caption = "Phone Number",
                    Values = new string[] { "Value #1", "Value #2", "Value #3" }
                },
                new ContactInfo("Email")
                {
                    Caption = "E-mail",
                    Values = new string[] { "Value #1", "Value #2" }
                }
            };
            var mockContactService = new Mock<IContactsService>();
            mockContactService.Setup(x => x.GetContactsAsync()).ReturnsAsync(testContacts);
            var target = new ContactSection(mockContactService.Object);
            int expectedAddressValuesCount = 3,
                expectedEmailValuesCount = 2;

            var result = await target.InvokeAsync() as ViewViewComponentResult;

            Assert.IsType<ContactSectionViewModel>(result.ViewData.Model);
            var model = result.ViewData.Model as ContactSectionViewModel;
            Assert.NotNull(model.Message);
            Assert.IsAssignableFrom<ContactsViewModel>(model.Contacts);
            Assert.Equal("Address", model.Contacts.Address.Name);
            Assert.Equal("Address", model.Contacts.Address.Caption);
            Assert.Equal(expectedAddressValuesCount, model.Contacts.Address.Values.Count);
            Assert.Equal("Value #1", model.Contacts.Address.Values.First());
            Assert.Equal("Value #3", model.Contacts.Address.Values.Last());
            Assert.NotNull(model.Contacts.Phone);
            Assert.Equal("Email", model.Contacts.Email.Name);
            Assert.Equal("E-mail", model.Contacts.Email.Caption);
            Assert.Equal(expectedEmailValuesCount, model.Contacts.Email.Values.Count);
            Assert.Equal("Value #1", model.Contacts.Email.Values.First());
            Assert.Equal("Value #2", model.Contacts.Email.Values.Last());
        }
    }
}
