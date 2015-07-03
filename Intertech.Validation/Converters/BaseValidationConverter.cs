﻿using Intertech.Validation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class BaseValidationConverter
    {
        /// <summary>
        /// Does the given attribute match the type T passed in?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected bool IsMatch<T>(CustomAttributeData attr)
        {
            if (attr == null) return false;

            return String.Compare(attr.AttributeType.FullName, typeof(T).FullName) == 0;
        }

        /// <summary>
        /// Get a constructor argument at the given index from the given attribute.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="constructorIndex"></param>
        /// <returns></returns>
        protected string GetConstructorArgumentValue(CustomAttributeData attr, int constructorIndex)
        {
            string value = null;

            if (attr.ConstructorArguments != null && attr.ConstructorArguments.Count > constructorIndex)
            {
                value = attr.ConstructorArguments[constructorIndex].Value.ToString();
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
        protected string GetNamedArgumentValue(string propertyName, CustomAttributeData attr, string nameOfArgument, bool usePropertyNameIfNull = true)
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

        /// <summary>
        /// Get the error message for the given property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected string GetErrorMessage(string propertyName, CustomAttributeData attr, string resourceNamespace, string resourceAssemblyName)
        {
            // First see if the ErrorMessage property is there.
            var msg = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.ErrorMessage, false);
            if (string.IsNullOrWhiteSpace(msg))
            {
                var resourceName = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.ErrorMessageResourceName, false);
                if (!string.IsNullOrWhiteSpace(resourceName))
                {
                    var resourceTypeStr = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.ErrorMessageResourceType, false);
                    if (!string.IsNullOrWhiteSpace(resourceTypeStr))
                    {
                        // Get the message from the resource.
                        try
                        {
                            var rtype = TypeHelper.GetObjectType(resourceTypeStr, true, resourceNamespace, resourceAssemblyName);
                            var resName = rtype.GetProperty(resourceName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            msg = resName.GetValue(null) as string;
                        }
                        catch
                        {
                            msg = null;
                        }
                    }
                }
            }

            return msg;
        }

        protected Dictionary<string, object> SetRegularExpressionAAValidation(string propertyName, CustomAttributeData attr,
            string regex, string defaultMsgFormat, string resourceNamespace = null, string resourceAssemblyName = null)
        {
            var validations = new Dictionary<string, object>();

            validations.Add("ng-pattern", "/" + regex + "/");

            var displayName = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.Display);
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var msg = GetErrorMessage(propertyName, attr, resourceNamespace, resourceAssemblyName);
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = string.Format(defaultMsgFormat, displayName);
                }
                validations.Add("ng-pattern-msg", msg);
            }
            return validations;
        }

        protected Dictionary<string, object> SetMaxLengthAAValidation(string propertyName, CustomAttributeData attr,
            string length, string resourceNamespace = null, string resourceAssemblyName = null)
        {
            var validations = new Dictionary<string, object>();
            validations.Add("ng-maxlength", length);

            var displayName = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.Display);
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var msg = GetErrorMessage(propertyName, attr, resourceNamespace, resourceAssemblyName);
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = string.Format(DataAnnotationConstants.DefaultMaxLengthErrorMsg, displayName, length);
                }
                validations.Add("ng-maxlength-msg", msg);
            }
            return validations;
        }

        protected Dictionary<string, object> SetMinLengthAAValidation(string propertyName, CustomAttributeData attr,
            string length, string resourceNamespace = null, string resourceAssemblyName = null)
        {

            var validations = new Dictionary<string, object>();

            validations.Add("ng-minlength", length);

            var displayName = GetNamedArgumentValue(propertyName, attr, DataAnnotationConstants.Display);
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var msg = GetErrorMessage(propertyName, attr, resourceNamespace, resourceAssemblyName);
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = string.Format(DataAnnotationConstants.DefaultMinLengthErrorMsg, displayName, length);
                }
                validations.Add("ng-minlength-msg", msg);
            }
            return validations;
         }
    }
}
