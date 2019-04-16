using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Works
{
    /// <summary>
    /// Представляет собой общую логику страниц добавления нового и обновления существующего примера работы, выполненной компанией.
    /// </summary>
    public class WorksAddEditCore
    {
        private IFileService _fileService;

        public WorksAddEditCore(IFileService fileService) => _fileService = fileService;

        public async Task<WorkExample> CreateWorkExampleFromViewModelAsync(WorkExampleViewModel model, DomainUser creator)
        {
            var workExample = new WorkExample(model.Id, creator)
            {
                Name = model.Name,
                Category = model.Category,
                Description = model.Description,
                ImagePath = model.ImagePath
            };
            if (model.Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    await model.Image.CopyToAsync(stream);
                    string extension = Path.GetExtension(model.Image.FileName);
                    string fileName = _fileService.GenerateUniqueFileName(extension, "workexample-");
                    workExample.ImagePath = await _fileService.SaveImageAsync(fileName, stream);
                }
            }
            return workExample;
        }
    }
}
