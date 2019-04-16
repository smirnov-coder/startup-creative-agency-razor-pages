using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Components
{
    /// <summary>
    /// Компонент представления навигационного меню административной части сайта.
    /// </summary>
    [ViewComponent]
    public class NavMenu : ViewComponent
    {
        private IMessageService _messageService;

        public NavMenu(IMessageService messageService) => _messageService = messageService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Общие ссылки для всех пользователей.
            var model = new NavMenuViewModel
            {
                NavLinks = new List<NavLink>
                {
                    new NavLink { Href = "/MyProfile", Text = "My Profile" },
                    new NavLink { Href = "/Services", Text = "Services" },
                    new NavLink { Href = "/Users", Text = "Users" },
                    new NavLink { Href = "/Works", Text = "Works" },
                    new NavLink { Href = "/Blog", Text = "Blog" },
                    new NavLink { Href = "/Brands", Text = "Brands" },
                    new NavLink { Href = "/Testimonials", Text = "Testimonials" }
                }
            };
            // Для администратора в навигационном меню отображаются две дополнительные ссылки,
            // а также количество новых непрочитанных сообщений от пользователей.
            if (User.IsInRole("Administrator"))
            {
                model.NavLinks.Add(new NavLink { Href = "/Contacts", Text = "Contacts" });
                model.NavLinks.Add(new NavLink { Href = "/Messages", Text = "Messages" });
                model.NewMessagesCount = (await _messageService.GetMessagesAsync())
                    .Where(message => !message.IsRead)
                    .Count();
            }
            return View(model);
        }
    }
}
