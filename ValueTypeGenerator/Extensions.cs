using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValueTypeGenerator
{
    internal static class Extensions
    {
        internal const string Validations = @"using System;

namespace ValueTypeGenerator.Validation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueTypeAttribute : Attribute { public ValueTypeAttribute() : base() { } }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IncludeValidationAttribute : Attribute { public IncludeValidationAttribute() : base() { } }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ExludeValidationAttribute : Attribute { public ExludeValidationAttribute() : base() { } }

    public interface IValueType { void Validate(); }

    public interface IValueValidator<T> { T GetValidationState(); }
}";

        private static IEnumerable<AttributeSyntax> Filter(SyntaxList<AttributeListSyntax> attributeList)
        {
            return attributeList.SelectMany(x => x.Attributes);
        }

        internal static bool ImplementsValueType(this ClassDeclarationSyntax source)
        {
            return !(source.BaseList is null)
                && source.BaseList.Types
                    .Any(baseType => baseType.ToString().Equals("IValueType"));
        }

        internal static bool HasValidationAttribute(this SyntaxList<AttributeListSyntax> attributeList)
        {
            return Filter(attributeList).Any(x =>
            {
                string attributeName = x.Name.ToString();
                return attributeName == "ValueTypeGenerator.Validation.ValueTypeAttribute"
                    || attributeName == "ValueTypeGenerator.Validation.ValueType"
                    || attributeName == "ValueTypeAttribute"
                    || attributeName == "ValueType";
            });
        }

        internal static List<PropertyDeclarationSyntax> GetProperties(this ClassDeclarationSyntax classSyntax)
        {
            return classSyntax.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(x =>
                        x.Modifiers.Any(y => y.Text == "public")
                        && Filter(x.AttributeLists).Any(IncludeOrExclude))
                .ToList();

            bool IncludeOrExclude(AttributeSyntax attribute)
            {
                string attributeName = attribute.Name.ToString();
                return attributeName != "ValueTypeGenerator.Validation.ExludeValidationAttribute"
                    || attributeName != "ValueTypeGenerator.Validation.ExludeValidation"
                    || attributeName != "ExludeValidationAttribute"
                    || attributeName != "ExludeValidation"

                    || attributeName == "ValueTypeGenerator.Validation.IncludeValidationAttribute"
                    || attributeName == "ValueTypeGenerator.Validation.IncludeValidation"
                    || attributeName == "IncludeValidationAttribute"
                    || attributeName == "IncludeValidation";
            }
        }
    }
}