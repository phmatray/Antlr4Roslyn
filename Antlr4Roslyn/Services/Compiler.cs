using Antlr4.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Antlr4Roslyn.Services;

public class Compiler
{
    public void Compile(string input)
    {
        // Étape 1 : Parser le code source avec ANTLR
        var inputStream = new AntlrInputStream(input);
        var lexer = new SimpleLexer(inputStream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new SimpleParser(tokens);
        var context = parser.program();

        // Étape 2 : Visiter l'arbre pour construire l'AST via Roslyn
        var visitor = new AntlrToRoslynVisitor();
        ExpressionSyntax expression = visitor.Visit(context);

        // Étape 3 : Générer le programme complet avec Roslyn
        var generator = new ProgramGenerator();
        CompilationUnitSyntax program = generator.GenerateProgram(expression);
        
        // Étape 4 : Compiler le programme avec Roslyn
        var compilerService = new CompilationService();
        compilerService.CompileAndExecute(program);
    }
}