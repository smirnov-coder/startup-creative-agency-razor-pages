using System;
using System.Collections.Generic;
using System.Text;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Unit.PageModels
{
    class TestViewModel
    {
        public int Id { get; set; }

        public TestViewModel() { }

        public TestViewModel(int id) => Id = id;
    }
}
