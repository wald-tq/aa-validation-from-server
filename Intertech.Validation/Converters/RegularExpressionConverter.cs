using Intertech.Validation.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class RegularExpressionConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<RegularExpressionAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            var pattern = GetConstructorArgumentValue(attr, 0);
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                return SetRegularExpressionAAValidation(propertyName, attr,
                    pattern, DataAnnotationConstants.DefaultRegexErrorMsg, resourceNamespace, resourceAssemblyName);
            }
            return new Dictionary<string, object>();
        }
    }
}
