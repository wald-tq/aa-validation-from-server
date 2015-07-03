using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Intertech.Validation.Constants;

namespace Intertech.Validation.Converters
{
    public class CreditCardConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<CreditCardAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            return SetRegularExpressionAAValidation(propertyName, attr,
                RegexConstants.CreditCard, DataAnnotationConstants.DefaultCreditCardErrorMsg, resourceNamespace, resourceAssemblyName);
        }
    }
}
