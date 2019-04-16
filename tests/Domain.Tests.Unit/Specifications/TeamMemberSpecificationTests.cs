using System;
using System.Collections.Generic;
using System.Linq;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Domain.Tests.Unit.Specifications
{
    public class TeamMemberSpecificationTests
    {
        [Fact]
        public void Criteria_Good()
        {
            var target = new TeamMemberSpecification();

            var result = GetTestUserCollection()
                .AsQueryable()
                .Where(target.Criteria)
                .ToList();

            Assert.Equal(3, result.Count());
            Assert.Equal(101, result.First().Id);
            Assert.Equal(103, result.Last().Id);
            Assert.DoesNotContain(result, x => x.Id == 104);
        }

        [Fact]
        public void LazyLoading_Good()
        {
            var target = new TeamMemberSpecification();

            var result = GetTestUserCollection()
                .AsQueryable()
                .Where(target.Criteria)
                .ToList();

            Assert.Equal(3, result.Count());
            Assert.NotNull(result.First().CreatedBy);
            Assert.NotNull(result.First().Identity);
            Assert.NotNull(result.First().Profile);
            Assert.Equal(4, result.First().Profile.SocialLinks.Count);
            Assert.Equal("Name #1", result.First().Identity.UserName);
            Assert.Equal("Facebook", result.First().Profile.SocialLinks.First().NetworkName);
            Assert.Equal("Linkedin", result.First().Profile.SocialLinks.Last().NetworkName);
            Assert.True(result.First().Profile.SocialLinks.All(x => !string.IsNullOrWhiteSpace(x.Url)));
        }

        private IList<DomainUser> GetTestUserCollection()
        {
            var creator = new DomainUser(new UserIdentity());
            var users = new List<DomainUser>
            {
                new DomainUser(101, new UserIdentity("Name #1", "Email #1"), new UserProfile("FirstName #1", "LastName #1", 
                    "Job #1", "Path #1"), creator),
                new DomainUser(102, new UserIdentity("Name #2", "Email #2"), new UserProfile("FirstName #2", "LastName #2",
                    "Job #2", "Path #2"), creator),
                new DomainUser(103, new UserIdentity("Name #3", "Email #3"), new UserProfile("FirstName #3", "LastName #3",
                    "Job #3", "Path #3"), creator),
                new DomainUser(104, new UserIdentity("Name #4", "Email #4"), new UserProfile("FirstName #4", "LastName #4",
                    "Job #4", "Path #4"), creator)
            };
            foreach (var user in users)
            {
                if (user.Id != 104)
                {
                    user.Profile.AddSocialLinks(GetTestSocialLinkCollection());
                    user.Profile.ChangeDisplayStatus(true);
                }
            }
            return users;
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
