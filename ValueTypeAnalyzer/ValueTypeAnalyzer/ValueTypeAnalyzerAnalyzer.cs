using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ValueTypeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValueTypeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ValueTypeAnalyzer";

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Design";

        private static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get => ImmutableArray.Create(Rule); }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
            context.EnableConcurrentExecution();

            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            //context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
        }

        //private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context) 
        //{
        //    if(!(context.Node is ClassDeclarationSyntax classDeclaration))
        //    {
        //        return;
        //    }
        //}

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            if (!(context.Symbol is INamedTypeSymbol namedTypeSymbol))
            {
                return;
            }

            if (!HasValidationAttribute(namedTypeSymbol.GetAttributes()))
            {
                return;
            }

            if (ImplementsValueType(namedTypeSymbol))
            {
                return;
            }

            // For all such symbols, produce a diagnostic.
            var diagnostic =
                Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }

        private bool HasValidationAttribute(ImmutableArray<AttributeData> attributeList)
        {
            const string longAttributeName = "ValueTypeAttribute";
            const string shortAttributeName = "ValueType";

            return attributeList.Any(x =>
               x.AttributeClass.Name == longAttributeName
            || x.AttributeClass.Name == shortAttributeName);
        }

        private bool ImplementsValueType(INamedTypeSymbol source)
        {
            return source.AllInterfaces
                .Any(iface => iface.Name == "IValueType");
        }
    }
}
