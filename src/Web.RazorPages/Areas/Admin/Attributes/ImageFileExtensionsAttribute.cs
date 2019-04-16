using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Attributes
{
    /// <summary>
    /// Указывает, что <see cref="IFormFile"/> должен быть файлом изображения с одним из допустимых 
    /// расширений: ".jpg", ".jpeg", ".png", ".gif".
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ImageFileExtensionsAttribute : ValidationAttribute
    {
        private FileExtensionsAttribute _extensionsAttribute = new FileExtensionsAttribute();

        public override bool IsValid(object value)
        {
            if (value != null && value is IFormFile)
            {
                var formFile = value as IFormFile;
                if (!_extensionsAttribute.IsValid(formFile.FileName))
                {
                    ErrorMessage = _extensionsAttribute.ErrorMessage;
                    return false;
                }
            }
            return true;
        }
    }
}
