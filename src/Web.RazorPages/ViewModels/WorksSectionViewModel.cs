using System.Collections.Generic;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Web.RazorPages.ViewModels
{
    public class WorksSectionViewModel
    {
        public IList<WorkExample> WorkExamples { get; set; }
        public IList<string> Categories { get; set; }
    }
}
