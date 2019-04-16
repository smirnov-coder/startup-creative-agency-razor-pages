using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.Attributes
{
    /// <summary>
    /// Указывает, что должно быть предоставлено не пустое значение хотя бы для одного из двух свойств.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AtLeastOneOfTwoAttribute : ValidationAttribute
    {
        public string OtherProperty { get; }

        public AtLeastOneOfTwoAttribute(string otherProperty) => OtherProperty = otherProperty;

        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null || !IsObjectNullOrEmptyString(value))
                return ValidationResult.Success;

            var model = validationContext.ObjectInstance;
            var propertyInfo = model.GetType().GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var propertyValue = propertyInfo.GetValue(model);
                if (propertyValue != null || !IsObjectNullOrEmptyString(propertyValue))
                    return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }

        private bool IsObjectNullOrEmptyString(object value)
        {
            if (value == null || (value.GetType() == typeof(string) && string.IsNullOrWhiteSpace(value as string)))
                return true;
            return false;
        }
    }
}
