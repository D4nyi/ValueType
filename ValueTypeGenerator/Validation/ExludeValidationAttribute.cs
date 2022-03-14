using System;

namespace ValueTypeGenerator.Validation
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ExludeValidationAttribute : Attribute
    {
        public ExludeValidationAttribute() : base() { }
    }
}
