using Intertech.Validation.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class RangeConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<RangeAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr)
        {
            var validations = new Dictionary<string, object>();
            
            var minimum = GetConstructorArgumentValue(attr, 0);
            var maximum = GetConstructorArgumentValue(attr, 1);
            
            if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
            {
                validations.Add("min", minimum);

                var displayName = base.GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.Display);
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    var msg = GetErrorMessage(propertyName, attr);
                    if (msg != null)
                    {
                        validations.Add("min-msg", msg);
                        validations.Add("max-msg", msg);

                    }
                    validations.Add("max", maximum);
                }
            }
            return validations;
        }
    }
}
