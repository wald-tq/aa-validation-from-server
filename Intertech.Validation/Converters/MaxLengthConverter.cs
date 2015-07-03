using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class MaxLengthConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<MaxLengthAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            var validations = new Dictionary<string, object>();

            var length = GetConstructorArgumentValue(attr, 0);

            return SetMaxLengthAAValidation(propertyName, attr, length, resourceNamespace, resourceAssemblyName);
        }
    }
}
