using System.Collections.Generic;

namespace Intertech.Validation
{
    /// <summary>
    /// Parameters for the GetValidations method to reduce complexity of the method signature.
    /// The old method signature will be backwards compatible.
    /// </summary>
    public class GetValidationsParms
    {
        public GetValidationsParms() { }

        public GetValidationsParms(string dtoObjectName, string jsonObjectName)
        {
            DtoObjectName = dtoObjectName;
            JsonObjectName = jsonObjectName;
        }

        /// <summary>
        /// Basically a copy-constructor
        /// </summary>
        public GetValidationsParms(GetValidationsParms p)
        {
            this.ValidationObject = p.ValidationObject;
            this.DtoObjectName = p.DtoObjectName;
            this.JsonObjectName = p.JsonObjectName;
            this.DtoAlternateNamespace = p.DtoAlternateNamespace;
            this.DtoAssemblyNames = p.DtoAssemblyNames;
            this.ResourceNamespace = p.ResourceNamespace;
            this.ResourceAssemblyName = p.ResourceAssemblyName;
        }

        public object ValidationObject { get; set; }

        public string DtoObjectName { get; set; }

        public string JsonObjectName { get; set; }

        public string DtoAlternateNamespace { get; set; }

        public List<string> DtoAssemblyNames { get; set; }

        public string ResourceNamespace { get; set; }

        public string ResourceAssemblyName { get; set; }
    }
}
