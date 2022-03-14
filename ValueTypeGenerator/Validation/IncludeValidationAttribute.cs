using System;

namespace ValueTypeGenerator.Validation
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IncludeValidationAttribute : Attribute
    {
        public IncludeValidationAttribute() : base() { }
    }
}
