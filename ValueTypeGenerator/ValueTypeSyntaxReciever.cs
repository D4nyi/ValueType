using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValueTypeGenerator
{
    internal sealed class ValueTypeSyntaxReciever : ISyntaxReceiver
    {
        private readonly List<ClassDeclarationSyntax> valueTypes;

        internal IReadOnlyCollection<ClassDeclarationSyntax> ValueTypes { get => valueTypes; }

        public ValueTypeSyntaxReciever()
        {
            valueTypes = new List<ClassDeclarationSyntax>();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
                && classDeclarationSyntax.AttributeLists.HasValidationAttribute()
                && classDeclarationSyntax.ImplementsValueType())
            {
                valueTypes.Add(classDeclarationSyntax);
            }
        }
    }
}
