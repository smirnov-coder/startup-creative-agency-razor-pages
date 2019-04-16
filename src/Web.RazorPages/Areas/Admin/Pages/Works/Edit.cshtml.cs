using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages.Works
{
    public class EditModel : EditPageModelBase<WorkExampleViewModel, WorkExample, int>
    {
        private readonly IWorkExampleService _workExampleService;
        private readonly IFileService _fileService;
        private readonly WorksAddEditCore _core;

        public EditModel(IWorkExampleService workExampleService, IFileService fileService, IUserService userService) : base(userService)
        {
            _workExampleService = workExampleService;
            _fileService = fileService;
            _core = new WorksAddEditCore(_fileService);
            RedirectUrl = "/works";
        }

        protected override async Task<WorkExample> CreateEntityFromModelAsync(WorkExampleViewModel model, DomainUser creator)
        {
            return await _core.CreateWorkExampleFromViewModelAsync(model, creator);
        }

        protected override WorkExampleViewModel CreateModelFromEntity(WorkExample entity)
        {
            var result = new WorkExampleViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Category = entity.Category,
                Description = entity.Description,
                ImagePath = Url.Content(entity.ImagePath)
            };
            return result;
        }

        protected override async Task<WorkExample> PerformGetSingleAsync(int entityId)
        {
            return await _workExampleService.GetWorkExampleAsync(entityId);
        }

        protected override async Task PerformUpdateAsync(WorkExample entity)
        {
            await _workExampleService.UpdateWorkExampleAsync(entity);
        }
    }
}
