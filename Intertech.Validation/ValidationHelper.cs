using Intertech.Validation.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Intertech.Validation
{
    public class ValidationHelper
    {
        private static List<IValidationConverter> _converters;

        public List<IValidationConverter> Converters { get { return _converters; } }

        public static List<IErrorMessageFormatter> _errorMessageFormatter {
            get 
            { 
                return BaseValidationConverter._errorMessageFormatter; 
            }
            set
            {
                BaseValidationConverter._errorMessageFormatter = value;
            }
        }

        /// <summary>
        /// Constructor that initializes the list of IValidationConverters.
        /// </summary>
        public ValidationHelper()
        {
            if (_converters == null)
            {
                var types = GetAllValidationConverters();
                if (types != null)
                {
                    _converters = new List<IValidationConverter>();

                    foreach (var t in types)
                    {
                        var constructorInfo = t.GetConstructor(System.Type.EmptyTypes);
                        if (constructorInfo != null)
                        {
                            var vcObj = constructorInfo.Invoke(null);
                            if (vcObj != null)
                            {
                                _converters.Add(vcObj as IValidationConverter);
                            }
                        }
                    }
                }
            }
            if (_errorMessageFormatter == null)
            {
                _errorMessageFormatter = new List<IErrorMessageFormatter> {
                    new AttributeErrorMessageFormatter(),
                    new ResourceErrorMessageFormatter(),
                    new DefaultErrorMessageFormatter()
                };
            }

        }

        /// <summary>
        /// Get the validations for dtoObjectName and return the json object.
        /// </summary>
        /// <param name="dtoObjectName"></param>
        /// <param name="jsonObjectName"></param>
        /// <param name="assemblyNames">Names of assemblies to check</param>
        /// <returns></returns>
        public object GetValidations(string dtoObjectName, string alternateNamespace, params string[] assemblyNames)
        {
            var parms = new GetValidationsParms(dtoObjectName, "validations")
            {
                DtoAlternateNamespace = alternateNamespace,
                DtoAssemblyNames = new List<string>(assemblyNames),
            };

            return GetValidations(parms);
        }

        /// <summary>
        /// This function iterates over the properties of the Dto and adds the validation attributes to the
        /// JsonString.
        /// </summary>
        public Dictionary<string, object> GetValidations(GetValidationsParms parms)
        {
            if (parms == null)
                throw new ArgumentNullException("parms", "Expecting GetValidationParms in GetValidations call and they were not supplied.");

            var modelValidations = new Dictionary<string, object>();

            var dtoClass = TypeHelper.GetObjectType(parms.DtoObjectName, false, parms.DtoAlternateNamespace, parms.DtoAssemblyNames.ToArray());
            if (dtoClass == null)
            {
                var message = string.Format("DTO '{0}' not found.", parms.DtoObjectName);
                throw new Exception(message);
            }

            var properties = dtoClass.GetProperties();
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    var propertyValidations = new Dictionary<string, object>();

                    if (prop.CustomAttributes != null && prop.CustomAttributes.Count() > 0)
                    {
                        var validateSubclassProp = Attribute.IsDefined(prop, typeof(ValidateSubclass));
                        if(validateSubclassProp)
                        {
                            var nextParms = new GetValidationsParms(parms);
                            nextParms.DtoObjectName = prop.PropertyType.Name;
                            nextParms.JsonObjectName = prop.Name;

                            modelValidations = modelValidations.Concat(GetValidations(nextParms)).ToDictionary(x => x.Key, x => x.Value);
                        }

                        foreach (var attr in prop.CustomAttributes)
                        {
                            var converter = _converters.FirstOrDefault(vc => vc.IsAttributeMatch(attr));
                            if (converter != null)
                            {
                                var ret = converter.Convert(prop.Name, attr);
                                propertyValidations = propertyValidations.Concat(ret).ToDictionary(x => x.Key, x => x.Value);
                            }
                        }

                    }
                    if (propertyValidations.Count > 0)
                    {
                        modelValidations.Add(prop.Name, propertyValidations);
                    }
                }
            }
            return new Dictionary<string, object>(){{parms.JsonObjectName, modelValidations}};
        }

        #region Private Methods

        /// <summary>
        /// Get all validation converters in this assembly.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetAllValidationConverters()
        {
            var valAssembly = typeof(IValidationConverter).Assembly;

            var registrations =
                from type in valAssembly.GetExportedTypes()
                where type.Namespace == "Intertech.Validation.Converters"
                where type.GetInterfaces().Any(t => t == typeof(IValidationConverter))
                select type;

            return registrations.AsEnumerable<Type>();
        }

        #endregion Private Methods
    }
}
