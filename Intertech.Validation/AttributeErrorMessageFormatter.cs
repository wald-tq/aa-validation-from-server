using Intertech.Validation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intertech.Validation
{
    public class AttributeErrorMessageFormatter: IErrorMessageFormatter
    {
        private static string NameOfArgument = "ErrorMessage";
        public string FormatErrorMessage(string propertyName, CustomAttributeData attr)
        {
            string value = null;

            if (attr.NamedArguments != null && attr.NamedArguments.Count > 0)
            {
                var namedArg = attr.NamedArguments.FirstOrDefault(na => na.MemberName == NameOfArgument);
                if (namedArg != null && namedArg.TypedValue != null && namedArg.TypedValue.Value != null)
                {
                    value = namedArg.TypedValue.Value.ToString();
                }
            }

            return value;
        }
    }

    public class DefaultErrorMessageFormatter: IErrorMessageFormatter
    {
        public static Dictionary<string, string> ErrorMessages = new Dictionary<string,string>{
            {"PhoneAttribute", "{0} is an invalid phone number."}, 
            {"UrlAttribute", "{0} is an invalid URL."},
            {"CreditCardAttribute", "{0} is an invalid credit card number."},
            {"EmailAddressAttribute", "{0} is an invalid email address."},
            {"MaxLengthAttribute", "{0} cannot be more than {1} characters."},
            {"MinLengthAttribute", "{0} cannot be less than {1} characters."},
            {"RequiredAttribute", "{0} is required."},
            {"RangeAttribute", "{0} must be between {1} and {2}."},
            {"StringLengthAttribute", "{0} must be between {1} and {2}."},
            {"Default", "{0} incorrect"}
        };

        public DefaultErrorMessageFormatter() { }
        public DefaultErrorMessageFormatter(Dictionary<string, string> messages)
        {
            ErrorMessages = messages;
        }

        public string FormatErrorMessage(string propertyName, CustomAttributeData attr)
        {
            try
            {
                var message = ErrorMessages[attr.AttributeType.Name];
                
                var arguments = new List<object>();
                arguments.Add(propertyName);
                if(attr.AttributeType.Name == "StringLengthAttribute")
                {
                    arguments.Add(attr.NamedArguments.Where(x => x.MemberName == "MinimumLength").Select(x => x.TypedValue.Value).First());
                }
                arguments.AddRange(attr.ConstructorArguments.Select(x => x.Value).Cast<object>());

                return String.Format(message, arguments.ToArray());
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    return String.Format(ErrorMessages["Default"], propertyName);
                }
                catch (KeyNotFoundException)
                {
                    return String.Format("{0} incorrect", propertyName);
                }
            }
        }
    }

    public class ResourceErrorMessageFormatter: IErrorMessageFormatter
    {
        public string ResourceNamespace { get; set; }
        public string ResourceAssemblyName { get; set; }

        public string FormatErrorMessage(string propertyName, CustomAttributeData attr)
        {
            string value = null;
            var resourceName = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.ErrorMessageResourceName, false);
            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                var resourceTypeStr = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.ErrorMessageResourceType, false);
                if (!string.IsNullOrWhiteSpace(resourceTypeStr))
                {
                    // Get the message from the resource.
                    try
                    {
                        var rtype = TypeHelper.GetObjectType(resourceTypeStr, true, ResourceNamespace, ResourceAssemblyName);
                        var resName = rtype.GetProperty(resourceName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        value = resName.GetValue(null) as string;
                    }
                    catch
                    {
                        value = null;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Get named argument value from the given attribute.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="attr"></param>
        /// <param name="nameOfArgument"></param>
        /// <param name="usePropertyNameIfNull"></param>
        /// <returns></returns>
        private static string GetNamedArgumentValue(string propertyName, CustomAttributeData attr, string nameOfArgument, bool usePropertyNameIfNull = true)
        {
            string value = null;

            if (attr.NamedArguments != null && attr.NamedArguments.Count > 0)
            {
                var namedArg = attr.NamedArguments.FirstOrDefault(na => na.MemberName == nameOfArgument);
                if (namedArg != null && namedArg.TypedValue != null && namedArg.TypedValue.Value != null)
                {
                    value = namedArg.TypedValue.Value.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(value) && usePropertyNameIfNull)
            {
                value = propertyName;
            }

            return value;
        }
    }



}
