using Intertech.Validation.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class StringLengthConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<StringLengthAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            var validations = new Dictionary<string, object>();

            var maxLength = GetConstructorArgumentValue(attr, 0);
            if (!string.IsNullOrWhiteSpace(maxLength))
            {
                var maxValidations = SetMaxLengthAAValidation(propertyName, attr, maxLength, resourceNamespace, resourceAssemblyName);
                validations = validations.Concat(maxValidations).ToDictionary(x => x.Key, x => x.Value);
            }

            var minLength = base.GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.MinimumLength, false);
            if (!string.IsNullOrWhiteSpace(minLength))
            {
                var minValidations = SetMinLengthAAValidation(propertyName, attr, minLength, resourceNamespace, resourceAssemblyName);
                validations = validations.Concat(minValidations).ToDictionary(x => x.Key, x => x.Value);
            }

            return validations;
        }
    }
}
