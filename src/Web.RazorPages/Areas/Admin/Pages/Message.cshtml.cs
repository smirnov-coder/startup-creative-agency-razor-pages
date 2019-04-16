using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class MessageModel : PageModel
    {
        private readonly IMessageService _messageService;

        public MessageModel(IMessageService messageService) => _messageService = messageService;

        public Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync([FromRoute]int id)
        {
            if ((Message = await _messageService.GetMessageAsync(id)) == null)
                return NotFound();
            await _messageService.UpdateMessageReadStatusAsync(id, true);
            return Page();
        }
    }
}
