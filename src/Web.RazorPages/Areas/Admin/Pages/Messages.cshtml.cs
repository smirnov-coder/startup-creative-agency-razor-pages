using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class MessagesModel : PageModel
    {
        private readonly IMessageService _messageService;

        public MessagesModel(IMessageService messageService) => _messageService = messageService;

        public IList<Message> Messages { get; set; }

        public async Task OnGetAsync() => Messages = await _messageService.GetMessagesAsync();

        public async Task<IActionResult> OnPostUnreadAsync(MessageViewModel[] messages)
        {
            return await PerformActionAsync(messages, async message =>
                await _messageService.UpdateMessageReadStatusAsync(message.Id, false), "updated");
        }

        public async Task<IActionResult> OnPostDeleteAsync(MessageViewModel[] messages)
        {
            return await PerformActionAsync(messages, async message =>
                await _messageService.DeleteMessageAsync(message.Id), "deleted");
        }

        private Task<IActionResult> PerformActionAsync(MessageViewModel[] messages, Action<MessageViewModel> action,
            string messagePart)
        {
            if (ModelState.IsValid)
            {
                var checkedMessages = messages.Where(message => message.IsRead);
                if (checkedMessages.Any())
                {
                    foreach (var message in checkedMessages)
                    {
                        action.Invoke(message);
                    }
                    this.SetDetails(OperationStatus.Success, $"A set of entities of type '{typeof(Message)}' has been " +
                        $"{messagePart} successfully.");
                }
            }
            else
            {
                this.SetDetails(OperationStatus.Error, $"Unable to perform operation. Reload the page and try again.");
            }
            return Task.FromResult<IActionResult>(RedirectToPage("/messages"));
        }
    }
}
