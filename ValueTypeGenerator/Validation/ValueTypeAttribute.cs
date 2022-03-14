using System;

namespace ValueTypeGenerator.Validation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueTypeAttribute : Attribute
    {
        public ValueTypeAttribute() { }
    }
}
