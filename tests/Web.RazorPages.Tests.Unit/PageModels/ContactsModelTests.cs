using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    public class ContactsModelTests
    {
        private Mock<IContactsService> _mockContactsService = new Mock<IContactsService>();
        private ContactsModel _target;

        public ContactsModelTests()
        {
            _target = new ContactsModel(_mockContactsService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task OnGetAsync_Good()
        {
            var testContacts = GetTestContacts();
            var testSocialLinks = GetTestSocialLinks();
            _mockContactsService.Setup(x => x.GetContactsAsync()).ReturnsAsync(testContacts);
            _mockContactsService.Setup(x => x.GetSocialLinksAsync()).ReturnsAsync(testSocialLinks);

            await _target.OnGetAsync();

            var contacts = _target.Contacts;
            Assert.Equal(testContacts.Count, contacts.Count);
            Assert.Equal(testContacts.First().Caption, contacts.First().Caption);
            Assert.Equal(testContacts.First().Name, contacts.First().Name);
            Assert.Equal(testContacts.First().Values.Count, contacts.First().Values.Count);
            Assert.Equal(testContacts.First().Values.First(), contacts.First().Values.First().Value);

            Assert.Equal(testContacts.Last().Caption, contacts.Last().Caption);
            Assert.Equal(testContacts.Last().Name, contacts.Last().Name);
            Assert.Equal(testContacts.Last().Values.Count, contacts.Last().Values.Count);
            Assert.Equal(testContacts.Last().Values.First(), contacts.Last().Values.First().Value);

            var socialLinks = _target.SocialLinks;
            Assert.Equal(testSocialLinks.Count, socialLinks.Count);
            Assert.Equal("Network #1", socialLinks.First().NetworkName);
            Assert.Equal(testSocialLinks["Network #1"], socialLinks.First().Url);

            Assert.Equal("Network #3", socialLinks.Last().NetworkName);
            Assert.Equal(testSocialLinks["Network #3"], socialLinks.Last().Url);
        }

        [Fact]
        public async Task OnPostAsync_Good()
        {
            _target.Contacts = GetTestContactsModel();
            _target.SocialLinks = GetTestSocialLinksModel();

            var actionResult = await _target.OnPostAsync();

            _mockContactsService.Verify(x => x.SaveContactsAsync(It.IsAny<IEnumerable<ContactInfo>>()), Times.Once());
            _mockContactsService.Verify(x => x.SaveSocialLinksAsync(It.IsAny<IDictionary<string, string>>()), Times.Once());

            Assert.IsAssignableFrom<RedirectToPageResult>(actionResult);
            var result = actionResult as RedirectToPageResult;
            Assert.Equal("/CONTACTS", result.PageName.ToUpper());
            var details = _target.TempData.Get<OperationDetails>("Details");
            Assert.Equal(OperationStatus.Success, details.Status);
            Assert.Equal("Company contacts saved successfully.", details.Message);
        }

        private IList<ContactInfo> GetTestContacts()
        {
            return new List<ContactInfo>
            {
                new ContactInfo("Name #1") { Caption = "Caption #1", Values = new string[] { "Value #1", "Value #2", "Value #3" } },
                new ContactInfo("Name #2") { Caption = "Caption #2", Values = new string[] { "Value #1", "Value #2", "Value #3" } },
                new ContactInfo("Name #3") { Caption = "Caption #3", Values = new string[] { "Value #1", "Value #2" } },
            };
        }

        private IDictionary<string, string> GetTestSocialLinks()
        {
            return new Dictionary<string, string>
            {
                ["Network #1"] = "Link #1",
                ["Network #2"] = "Link #2",
                ["Network #3"] = "Link #3",
            };
        }

        private IList<ContactInfoViewModel> GetTestContactsModel()
        {
            return GetTestContacts().Select(contact => new ContactInfoViewModel
            {
                Caption = contact.Caption,
                Name = contact.Name,
                Values = contact.Values.Select(x => new ContactValue { Value = x }).ToList()
            }).ToList();
        }

        private IList<SocialLinkViewModel> GetTestSocialLinksModel()
        {
            return GetTestSocialLinks().Select(socialLink => new SocialLinkViewModel
            {
                NetworkName = socialLink.Key,
                Url = socialLink.Value
            }).ToList();
        }
    }
}
