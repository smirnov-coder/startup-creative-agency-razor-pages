using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Web.RazorPages.Components;
using StartupCreativeAgency.Web.RazorPages.ViewModels;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.ViewComponents
{
    public class WorksSectionTests
    {
        [Fact]
        public async Task InvokeAsync_Good()
        {
            // Arrange
            var creator = new DomainUser(new UserIdentity(), new UserProfile());
            var works = new List<WorkExample>
            {
                new WorkExample(1, creator) { Category = "Category #1", Name = "Name #1", ImagePath = "Path #1", Description = "Description #1" },
                new WorkExample(2, creator) { Category = "Category #3", Name = "Name #2", ImagePath = "Path #2", Description = "Description #2" },
                new WorkExample(3, creator) { Category = "Category #1", Name = "Name #3", ImagePath = "Path #3", Description = "Description #3" },
                new WorkExample(4, creator) { Category = "Category #2", Name = "Name #4", ImagePath = "Path #4", Description = "Description #4" },
                new WorkExample(5, creator) { Category = "Category #1", Name = "Name #5", ImagePath = "Path #5", Description = "Description #5" }
            };
            var mockService = new Mock<IWorkExampleService>();
            mockService.Setup(x => x.GetWorkExamplesAsync()).ReturnsAsync(works);
            var target = new WorksSection(mockService.Object);
            int expectedCategoryCount = 4;

            // Act
            var result = await target.InvokeAsync() as ViewViewComponentResult;

            // Assert
            Assert.IsType<WorksSectionViewModel>(result.ViewData.Model);
            var model = result.ViewData.Model as WorksSectionViewModel;
            Assert.Equal(expectedCategoryCount, model.Categories.Count);
            Assert.Equal("*", model.Categories.First());
            Assert.Single((model.Categories as List<string>).FindAll(x => x == "Category #1"));
            Assert.Equal(works.Count, model.WorkExamples.Count);
            Assert.Equal("Category #1", model.WorkExamples.First().Category);
            Assert.Equal("Name #1", model.WorkExamples.First().Name);
            Assert.Equal("Path #1", model.WorkExamples.First().ImagePath);
            Assert.Equal("Description #1", model.WorkExamples.First().Description);
            Assert.Equal("Category #1", model.WorkExamples.Last().Category);
            Assert.Equal("Name #5", model.WorkExamples.Last().Name);
            Assert.Equal("Path #5", model.WorkExamples.Last().ImagePath);
            Assert.Equal("Description #5", model.WorkExamples.Last().Description);
        }
    }
}