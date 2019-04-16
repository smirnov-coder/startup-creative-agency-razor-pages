using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Controllers.Api
{
    /// <summary>
    /// Контроллер для работы в примерами выполненных компанией работ.
    /// </summary>
    public class WorkExamplesController : ApiControllerBase<WorkExample, int>
    {
        private readonly IWorkExampleService _workExampleService;

        public WorkExamplesController(IWorkExampleService workExampleService) => _workExampleService = workExampleService;

        protected override async Task<WorkExample> PerformGetAsync(int id) => await _workExampleService.GetWorkExampleAsync(id);

        protected override void PrepareEntityForReturn(WorkExample entity) => entity.ImagePath = Url.Content(entity.ImagePath);
    }
}
