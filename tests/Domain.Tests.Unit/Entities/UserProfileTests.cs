using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Entities
{
    public class UserProfileTests
    {
        [Fact]
        public void UpdatePersonalInfo_Good()
        {
            var target = new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1",
                GetTestSocialLinkCollection());

            Assert.Raises<EventArgs>(
                handler => target.UserInfoUpdated += handler, 
                handler => target.UserInfoUpdated -= handler,
                () => target.UpdatePersonalInfo("FirstName #2", "LastName #2", "Job #2", "Path #2"));

            Assert.Equal("FirstName #2", target.FirstName);
            Assert.Equal("LastName #2", target.LastName);
            Assert.Equal("Job #2", target.JobPosition);
            Assert.Equal("Path #2", target.PhotoFilePath);
            Assert.True(target.IsReadyForDisplay);
            Assert.False(target.DisplayAsTeamMember);
            Assert.NotEmpty(target.SocialLinks);
            Assert.Equal(4, target.SocialLinks.Count);
        }

        [Fact]
        public void UpdatePersonalInfo_Good_SaveDisplayAsTeamMemberFlag()
        {
            var target = new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1", 
                GetTestSocialLinkCollection());
            Assert.True(target.IsReadyForDisplay);
            Assert.False(target.DisplayAsTeamMember);
            target.ChangeDisplayStatus(true);
            Assert.True(target.DisplayAsTeamMember);

            target.UpdatePersonalInfo("FirstName #2", "LastName #2", "Job #2", "Path #2");

            Assert.True(target.IsReadyForDisplay);
            Assert.True(target.DisplayAsTeamMember);
        }

        [Fact]
        public void UpdatePersonalInfo_Good_LoseDisplayAsTeamMemberFlag()
        {
            var target = new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1", 
                GetTestSocialLinkCollection());
            Assert.True(target.IsReadyForDisplay);
            Assert.False(target.DisplayAsTeamMember);
            target.ChangeDisplayStatus(true);
            Assert.True(target.DisplayAsTeamMember);

            target.UpdatePersonalInfo("FirstName #2", "LastName #2", null, "Path #2");

            Assert.False(target.IsReadyForDisplay);
            Assert.False(target.DisplayAsTeamMember);
        }

        [Fact]
        public void AddSocialLinks_Good_AddNotExistingSocialLink()
        {
            var target = new UserProfile();
            Assert.Empty(target.SocialLinks);
            target.AddSocialLinks(GetTestSocialLinkCollection());
            Assert.Equal(4, target.SocialLinks.Count);
            int expectedSocialLinksCount = 5,
                expectedAddedCount = 1,
                actualAddedCount = 0;

            Assert.Raises<EventArgs>(
                handler => target.UserInfoUpdated += handler,
                handler => target.UserInfoUpdated -= handler,
                () => actualAddedCount = target.AddSocialLinks(
                new SocialLink("Test Name", "Test Url") 
            ));

            Assert.Equal(expectedAddedCount, actualAddedCount);
            Assert.Equal(expectedSocialLinksCount, target.SocialLinks.Count);
            Assert.Equal("Facebook", target.SocialLinks.First().NetworkName);
            Assert.Equal("Url #1", target.SocialLinks.First().Url);
            Assert.Equal("Test Name", target.SocialLinks.Last().NetworkName);
            Assert.Equal("Test Url", target.SocialLinks.Last().Url);
        }

        [Fact]
        public void AddSocialLinks_Good_AddExistingSocialLink()
        {
            var target = new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1",
                GetTestSocialLinkCollection());
            int expectedSocialLinksCount = 4, 
                addedCount = 0;

            Assert.Raises<EventArgs>(
                handler => target.UserInfoUpdated += handler,
                handler => target.UserInfoUpdated -= handler,
                () => addedCount = target.AddSocialLinks(new SocialLink("Facebook", "New Url")));

            Assert.Equal(0, addedCount);
            Assert.Equal(expectedSocialLinksCount, target.SocialLinks.Count);
            Assert.Equal("Facebook", target.SocialLinks.First().NetworkName);
            Assert.Equal("New Url", target.SocialLinks.First().Url);
            Assert.Equal("Linkedin", target.SocialLinks.Last().NetworkName);
            Assert.Equal("Url #4", target.SocialLinks.Last().Url);
        }

        [Fact]
        public void ClearSocialLinks_Good()
        {
            var target = new UserProfile();
            Assert.Empty(target.SocialLinks);
            target.AddSocialLinks(GetTestSocialLinkCollection());
            Assert.Equal(4, target.SocialLinks.Count);

            Assert.Raises<EventArgs>(
                handler => target.UserInfoUpdated += handler,
                handler => target.UserInfoUpdated -= handler,
                () => target.ClearSocialLinks());

            Assert.Empty(target.SocialLinks);
        }

        [Fact]
        public void ChangeDisplayStatus_Good()
        {
            var target = new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1", 
                GetTestSocialLinkCollection());
            Assert.True(target.IsReadyForDisplay);
            Assert.False(target.DisplayAsTeamMember);

            Assert.Raises<EventArgs>(
                handler => target.UserInfoUpdated += handler,
                handler => target.UserInfoUpdated -= handler,
                () => target.ChangeDisplayStatus(true));

            Assert.True(target.IsReadyForDisplay);
            Assert.True(target.DisplayAsTeamMember);
        }

        [Fact]
        public void ChangeDisplayStatus_Bad_InvalidOperationException()
        {
            var target = new UserProfile("FirstName #1", string.Empty,
                "Job #1", "Path #1", new SocialLink("Name #1", "Url #1"));
            Assert.False(target.IsReadyForDisplay);
            Assert.False(target.DisplayAsTeamMember);

            Assert.Throws<InvalidOperationException>(() => target.ChangeDisplayStatus(true));
        }

        private SocialLink[] GetTestSocialLinkCollection()
        {
            return new SocialLink[]
            {
                new SocialLink("Facebook", "Url #1"),
                new SocialLink("Twitter", "Url #2"),
                new SocialLink("GooglePlus", "Url #3"),
                new SocialLink("Linkedin", "Url #4")
            };
        }
    }
}
