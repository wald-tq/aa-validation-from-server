using System.Collections.Generic;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public interface IValidationConverter
    {
        bool IsAttributeMatch(CustomAttributeData attr);

        Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr);
    }
}
