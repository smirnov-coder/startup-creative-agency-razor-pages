using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Pages
{
    public class WorksModel : ListPageModelBase<WorkExample, int>
    {
        private readonly IWorkExampleService _workExampleService;

        public WorksModel(IWorkExampleService workExampleService)
        {
            _workExampleService = workExampleService;
            RedirectUrl = "/works";
        }

        protected override async Task PerformDeleteAsync(int entityId) => await _workExampleService.DeleteWorkExampleAsync(entityId);

        protected override async Task<IList<WorkExample>> PerformGetManyAsync() => await _workExampleService.GetWorkExamplesAsync();
    }
}
