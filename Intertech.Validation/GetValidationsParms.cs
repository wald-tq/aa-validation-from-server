using System.Collections.Generic;

namespace Intertech.Validation
{
    /// <summary>
    /// Parameters for the GetValidations method to reduce complexity of the method signature.
    /// The old method signature will be backwards compatible.
    /// </summary>
    public class GetValidationsParms
    {
        public GetValidationsParms(string dtoObjectName, string jsonObjectName)
        {
            DtoObjectName = dtoObjectName;
            JsonObjectName = jsonObjectName;
        }

        public object ValidationObject { get; set; }

        public string DtoObjectName { get; set; }

        public string JsonObjectName { get; set; }

        public string DtoAlternateNamespace { get; set; }

        public List<string> DtoAssemblyNames { get; set; }

        public string ResourceNamespace { get; set; }

        public string ResourceAssemblyName { get; set; }

        public bool UseCamelCaseForProperties { get; set; }
    }
}
