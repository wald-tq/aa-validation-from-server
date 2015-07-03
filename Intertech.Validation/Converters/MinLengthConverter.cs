using Intertech.Validation.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intertech.Validation.Converters
{
    public class MinLengthConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<MinLengthAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            var validations = new Dictionary<string, object>();
            var length = GetConstructorArgumentValue(attr, 0);

            return SetMinLengthAAValidation(propertyName, attr, length, resourceNamespace, resourceAssemblyName);
        }
    }
}
