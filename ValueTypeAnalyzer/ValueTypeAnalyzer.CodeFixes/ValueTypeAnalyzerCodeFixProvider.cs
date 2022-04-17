using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using ValueTypeAnalyzer.CodeFixes;

namespace ValueTypeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ValueTypeAnalyzerCodeFixProvider)), Shared]
    public class ValueTypeAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ValueTypeAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => AddInterfaceToBaseList(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private static async Task<Solution> AddInterfaceToBaseList(Document document, ClassDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            SimpleBaseTypeSyntax iValueType = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IValueType"));
            ClassDeclarationSyntax newClass = typeDecl.AddBaseListTypes(iValueType).AddMembers(CreateMethod());

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot = root.ReplaceNode(typeDecl, newClass);

            return document.WithSyntaxRoot(newRoot).Project.Solution;
        }

        private static MemberDeclarationSyntax CreateMethod()
        {
            CSharpParseOptions options = new CSharpParseOptions(LanguageVersion.CSharp7_3);
            return SyntaxFactory.ParseMemberDeclaration(@"
        public void Validate()
        {
            throw new NotImplementedException();
        }
", 0, options, true);
        }
    }
}
