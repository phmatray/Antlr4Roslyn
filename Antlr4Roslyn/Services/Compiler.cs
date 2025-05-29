using Antlr4.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Antlr4Roslyn.Services;

public class Compiler
{
    public void Compile(string input)
    {
        // Step 1: Parse the source code with ANTLR
        var inputStream = new AntlrInputStream(input);
        var lexer = new SimpleLexer(inputStream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new SimpleParser(tokens);

        // Step 2: Visit the tree to build the Roslyn AST
        var visitor = new AntlrToRoslynVisitor();
        var context = parser.program();
        SyntaxNode programNode = visitor.VisitProgram(context);

        // Step 3: Generate the complete program with Roslyn
        var generator = new ProgramGenerator();
        CompilationUnitSyntax program = generator.GenerateProgram(programNode);
        
        // Step 4: Compile the program with Roslyn
        var compilerService = new CompilationService();
        compilerService.CompileAndExecute(program);
    }
}