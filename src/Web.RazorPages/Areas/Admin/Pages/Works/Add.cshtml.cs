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
    public class AddModel : AddPageModelBase<WorkExampleViewModel, WorkExample, int>
    {
        private readonly IWorkExampleService _workExampleService;
        private readonly IFileService _fileService;
        private readonly WorksAddEditCore _core;

        public AddModel(IWorkExampleService workExampleService, IFileService fileService, IUserService userService) : base(userService)
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

        protected override async Task<WorkExample> PerformAddAsync(WorkExample entity)
        {
            return await _workExampleService.AddWorkExampleAsync(entity);
        }
    }
}
