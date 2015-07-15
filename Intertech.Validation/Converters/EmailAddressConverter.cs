﻿using Intertech.Validation.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Intertech.Validation.Converters
{
    public class EmailAddressConverter : BaseValidationConverter, IValidationConverter
    {
        public bool IsAttributeMatch(CustomAttributeData attr)
        {
            return IsMatch<EmailAddressAttribute>(attr);
        }

        public Dictionary<string, object> Convert(string propertyName, CustomAttributeData attr)
        {
            return SetRegularExpressionAAValidation(propertyName, attr, 
                RegexConstants.Email);
        }
    }
}
