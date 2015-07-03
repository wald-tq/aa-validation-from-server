using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intertech.Validation.Converters
{
    public interface IValidationConverter
    {
        bool IsAttributeMatch(CustomAttributeData attr);

        Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName);
    }
}
