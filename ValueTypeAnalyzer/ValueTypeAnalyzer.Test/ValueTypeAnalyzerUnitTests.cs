using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = ValueTypeAnalyzer.Test.CSharpCodeFixVerifier<
    ValueTypeAnalyzer.ValueTypeAnalyzerAnalyzer,
    ValueTypeAnalyzer.ValueTypeAnalyzerCodeFixProvider>;

namespace ValueTypeAnalyzer.Test
{
    [TestClass]
    public class ValueTypeAnalyzerUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task EmptyFile_NoDiagnostic()
        {
            string test = "";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task AttributeAddedWithoutInterface_NoDiagnostic()
        {
            string test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueTypeAttribute : Attribute
    {
        public ValueTypeAttribute() { }
    }
    public interface IValueType { void Validate(); }

    [ValueType]
    class Temperature
    {
    }
}";

            string fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueTypeAttribute : Attribute
    {
        public ValueTypeAttribute() { }
    }
    public interface IValueType { void Validate(); }

    [ValueType]
    class Temperature
: IValueType
    {
        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}";

            DiagnosticResult expected = VerifyCS.Diagnostic(ValueTypeAnalyzerAnalyzer.DiagnosticId)
                .WithSpan(19, 11, 19, 22)
                .WithArguments("Temperature");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
