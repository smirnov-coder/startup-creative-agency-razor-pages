using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class MessageViewModel
    {
        [Required, HiddenInput]
        public int Id { get; set; }

        [Required]
        public bool IsRead { get; set; }
    }
}
