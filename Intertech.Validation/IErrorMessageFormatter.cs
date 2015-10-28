using System.Reflection;

namespace Intertech.Validation
{
    public interface IErrorMessageFormatter
    {
        string FormatErrorMessage(string propertyName, CustomAttributeData attr);
    }
}
