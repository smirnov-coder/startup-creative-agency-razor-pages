using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class ContactInfoViewModel
    {
        [Required, HiddenInput]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Caption { get; set; }

        public IList<ContactValue> Values { get; set; }
    }

    public class ContactValue
    {
        [Required]
        [StringLength(100)]
        public string Value { get; set; }
    }
}
