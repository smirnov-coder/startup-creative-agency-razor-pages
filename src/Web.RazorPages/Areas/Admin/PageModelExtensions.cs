using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using StartupCreativeAgency.Web.RazorPages.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin
{
    public static class PageModelExtensions
    {
        /// <summary>
        /// Добавляет информацию об операции в коллекцию <see cref="ITempDataDictionary"/> страницы.
        /// </summary>
        /// <param name="status">Статус операции.</param>
        /// <param name="message">Текст сообщения.</param>
        public static void SetDetails(this PageModel page, OperationStatus status, string message)
        {
            page.TempData.Set("Details", new OperationDetails
            {
                Status = status,
                Message = message
            });
        }
    }
}
