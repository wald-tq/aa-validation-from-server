using Intertech.Validation.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class RequiredConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<RequiredAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr)
        {
            var validations = new Dictionary<string, object>();

            validations.Add("required", true);

            var displayName = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.Display);
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var msg = GetErrorMessage(propertyName, attr);
                if (msg != null)
                {
                    validations.Add("required-msg", msg);
                }
            }
            return validations;
        }
    }
}
