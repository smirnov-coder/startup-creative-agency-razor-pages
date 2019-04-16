using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.RazorPages.Controllers.Api;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.ViewModels;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.Controllers
{
    public class MessageControllerTests
    {
        private Mock<IMessageService> _mockMessageService = new Mock<IMessageService>();
        private MessagesController _target;

        public MessageControllerTests()
        {
            _target = new MessagesController(_mockMessageService.Object);
        }

        [Fact]
        public async Task SaveAsync_Good()
        {
            var result = await _target.SaveAsync(new MessageViewModel());

            _mockMessageService.Verify(x => x.SaveMessageAsync(It.IsAny<Message>()), Times.Once());
            Assert.IsAssignableFrom<OkObjectResult>(result);
            string message = (result as OkObjectResult).Value as string;
            Assert.Equal("Thank you for your message!", message);
        }

        [Fact]
        public async Task SaveAsync_Bad_ValidationProblem()
        {
            _target.ModelState.AddModelError("Test Key", "Test Error");

            var result = await _target.SaveAsync(new MessageViewModel());

            _mockMessageService.Verify(x => x.SaveMessageAsync(It.IsAny<Message>()), Times.Never);
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var details = (result as BadRequestObjectResult).Value as ValidationProblemDetails;
            Assert.Single(details.Errors);
            Assert.True(details.Errors.ContainsKey("Test Key"));
            Assert.Equal("Test Error", details.Errors["Test Key"].First());
        }

        [Fact]
        public async Task SaveAsync_Bad_BadRequest()
        {
            _mockMessageService.Setup(x => x.SaveMessageAsync(It.IsAny<Message>())).ThrowsAsync(new Exception());

            var result = await _target.SaveAsync(new MessageViewModel());

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            string message = (result as BadRequestObjectResult).Value as string;
            Assert.Equal("Oops! We are sorry. Something went wrong on the server side. Try again later.", message);
        }
    }
}
