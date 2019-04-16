using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.ViewModels;

namespace Web.RazorPages.Controllers.Api
{
    /// <summary>
    /// Контроллер для работы с сообщениями от пользователей.
    /// </summary>
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService) => _messageService = messageService;
        
        //
        // POST api/Messages
        //
        // Сохраняет новое сообщение от пользователя.
        [HttpPost]
        public async Task<IActionResult> SaveAsync(MessageViewModel message)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(new ValidationProblemDetails(ModelState));
            try
            {
                var entity = new Message()
                {
                    Name = message.Name,
                    Email = message.Email,
                    Company = message.Company,
                    Subject = message.Subject,
                    Text = message.Text,
                    IPAddress = Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };
                await _messageService.SaveMessageAsync(entity);
                return Ok("Thank you for your message!");
            }
            catch
            {
                return BadRequest("Oops! We are sorry. Something went wrong on the server side. Try again later.");
            }
        }
    }
}
