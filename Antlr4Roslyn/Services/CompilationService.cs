using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Antlr4Roslyn.Services;

public class CompilationService
{
    public void CompileAndExecute(CompilationUnitSyntax compilationUnit)
    {
        var compilation = CreateCompilation(compilationUnit);
        using var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
            Console.WriteLine("Failed to compile the program.");
            ReportDiagnostics(emitResult.Diagnostics);
            Console.WriteLine(compilationUnit.ToFullString());
        }
        else
        {
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var type = assembly.GetType("GeneratedProgram.Program");
            if (type == null)
            {
                Console.WriteLine("Failed to load the Program class.");
                return;
            }
            var method = type.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                Console.WriteLine("Failed to load the Main method.");
                return;
            }
            method.Invoke(null, null);
        }
    }

    private CSharpCompilation CreateCompilation(CompilationUnitSyntax program)
    {
        var syntaxTree = SyntaxFactory.SyntaxTree(program);
        
        var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
        if (dotNetCoreDir == null)
        {
            throw new NotSupportedException("Failed to locate the .NET Core directory.");
        }
        
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // System.Private.CoreLib.dll
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // System.Console.dll
        };

        return CSharpCompilation.Create("GeneratedProgram")
            .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTree);
    }

    private void ReportDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine(diagnostic.ToString());
        }
    }
}
