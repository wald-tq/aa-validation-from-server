using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Intertech.Validation.Converters;
using System.Globalization;

namespace Intertech.Validation
{
    public class ValidationHelper
    {
        private static List<IValidationConverter> _converters;
        public List<IValidationConverter> Converters { get { return _converters; } }

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
        }

        /// <summary>
        /// Get the validations for dtoObjectName and return the json object.
        /// </summary>
        /// <param name="dtoObjectName"></param>
        /// <param name="jsonObjectName"></param>
        /// <param name="assemblyNames">Names of assemblies to check</param>
        /// <returns></returns>
        public object GetValidations(string dtoObjectName, string jsonObjectName, string alternateNamespace, bool useCamelCaseForProperties, params string[] assemblyNames)
        {
            var parms = new GetValidationsParms(dtoObjectName, jsonObjectName)
            {
                DtoAlternateNamespace = alternateNamespace,
                DtoAssemblyNames = new List<string>(assemblyNames),
                UseCamelCaseForProperties = useCamelCaseForProperties
            };

            return GetValidations(parms);
        }

        /// <summary>
        /// Get the validations for the given parms.
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public object GetValidations(GetValidationsParms parms)
        {
            if (parms == null)
                throw new ArgumentNullException("parms", "Expecting GetValidationParms in GetValidations call and they were not supplied.");

            return GetValidationsForDto(parms);
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

        /// <summary>
        /// This function iterates over the properties of the Dto and adds the validation attributes to the
        /// JsonString.
        /// </summary>
        private object GetValidationsForDto(GetValidationsParms parms)
        {
            var modelValidations = new Dictionary<string, object>();
            
            //parms.JsonString.Append(parms.JsonObjectName + ": { ");

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
                        foreach (var attr in prop.CustomAttributes)
                        {
                            var converter = _converters.FirstOrDefault(vc => vc.IsAttributeMatch(attr));
                            if (converter != null)
                            {
                                var ret = converter.Convert(prop.Name, attr, parms.ResourceNamespace, parms.ResourceAssemblyName);
                                propertyValidations = propertyValidations.Concat(ret).ToDictionary(x => x.Key, x => x.Value);
                            }
                        }

                    }
                    modelValidations.Add(prop.Name, propertyValidations);
                }
            }
            parms.ValidationObject = new { validations = new Dictionary<string, object>() { { parms.JsonObjectName, modelValidations } } };

            return parms.ValidationObject;
        }

		private string CamelCaseProperty(string input)
		{
			if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
			{
				return input;
			}

			var sb = new StringBuilder();

			for (var i = 0; i < input.Length; ++i)
			{
				var flag = i + 1 < input.Length;
				if (i == 0 || !flag || char.IsUpper(input[i + 1]))
				{
					var ch = char.ToLower(input[i], CultureInfo.InvariantCulture);
					sb.Append(ch);
				}
				else
				{
					sb.Append(input.Substring(i));
					break;
				}
			}

			return sb.ToString();
		}



        #endregion Private Methods
    }
}
