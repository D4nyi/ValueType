using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using ValueTypeGenerator.Validation;

namespace ValueTypeGenerator
{
    internal static class Extensions
    {
        internal static bool ImplementsValueType(this ClassDeclarationSyntax source)
        {
            return !(source.BaseList is null)
                && source.BaseList.Types
                    .Any(baseType => baseType.ToString().StartsWith(nameof(IValueType)));
        }

        internal static bool HasValidationAttribute(this SyntaxList<AttributeListSyntax> attributeList)
        {
            string longAttributeName = nameof(ValueTypeAttribute);
            return attributeList.HasAttribute(longAttributeName);
        }

        internal static bool HasAttribute(this SyntaxList<AttributeListSyntax> attributeList, string longAttributeName)
        {
            string shortAttributeName = longAttributeName.Replace(nameof(Attribute), "");
            return attributeList.Any(x => x.Attributes.Any(y =>
            {
                string attributeName = y.Name.ToString();
                return attributeName == longAttributeName || attributeName == shortAttributeName;
            }));
        }

        internal static List<PropertyDeclarationSyntax> GetProperties(this ClassDeclarationSyntax classSyntax)
        {
            string excludeValidation = nameof(ExludeValidationAttribute);
            string includeValidation = nameof(IncludeValidationAttribute);

            return classSyntax.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(y => y.Text == "public")
                                                && !x.AttributeLists.HasAttribute(excludeValidation)
                                                || x.AttributeLists.HasAttribute(includeValidation))
                .ToList();
        }
    }
}
