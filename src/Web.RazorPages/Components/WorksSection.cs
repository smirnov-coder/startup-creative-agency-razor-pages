using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Web.RazorPages.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Components
{
    /// <summary>
    /// Компонент представления секции "Works" главной страницы.
    /// </summary>
    [ViewComponent]
    public class WorksSection : ViewComponent
    {
        private IWorkExampleService _workExampleService;

        public WorksSection(IWorkExampleService workExampleService) => _workExampleService = workExampleService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var workExamples = await _workExampleService.GetWorkExamplesAsync();
            var model = new WorksSectionViewModel
            {
                WorkExamples = workExamples,
                // Сформировать список категорий, на которые разбита коллекция примеров работ.
                // По значению категории выполняется сортировка отображаемых примеров работ.
                // "*" означает "все примеры работ".
                Categories = workExamples
                    .Select(workExample => workExample.Category)
                    .Distinct()
                    .Prepend("*")
                    .ToList()
            };
            return View(model);
        }
    }
}
