using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Components
{
    /// <summary>
    /// Компонент представления виджета информации о текущем аутентифицированном пользователе.
    /// Виджет состоит из фотографии пользователя, идентификационного имени пользователя и кнопки выхода.
    /// </summary>
    [ViewComponent]
    public class UserWidget : ViewComponent
    {
        private IUserService _userService;

        public UserWidget(IUserService userService) => _userService = userService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userService.GetUserAsync(User?.Identity?.Name);
            return View((object)user.Profile.PhotoFilePath);
        }
    }
}
