using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class NavMenuViewModel
    {
        public int NewMessagesCount { get; set; }

        public IList<NavLink> NavLinks { get; set; }
    }

    /// <summary>
    /// Навигационная ссылка.
    /// </summary>
    public class NavLink
    {
        /// <summary>
        /// Относительный адрес страницы.
        /// </summary>
        public string Href { get; set; }
        /// <summary>
        /// Отображаемый в ссылке текст.
        /// </summary>
        public string Text { get; set; }
    }
}
