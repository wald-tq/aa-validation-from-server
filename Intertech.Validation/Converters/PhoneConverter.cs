using Intertech.Validation.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class PhoneConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<PhoneAttribute>(attr);
        }
        
        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            return SetRegularExpressionAAValidation(propertyName, attr, 
                RegexConstants.Phone, DataAnnotationConstants.DefaultPhoneErrorMsg, resourceNamespace, resourceAssemblyName);
        }
    }
}
