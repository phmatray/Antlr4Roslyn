using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Antlr4Roslyn.Services;

public class ProgramGenerator
{
    public CompilationUnitSyntax GenerateProgram(SyntaxNode programNode)
    {
        BlockSyntax body;
        if (programNode is BlockSyntax block)
        {
            // Convert the last expression statement to a Console.WriteLine if it's not already a full statement
            var statements = ConvertBlockStatements(block.Statements.ToList());
            body = SyntaxFactory.Block(SyntaxFactory.List(statements));
        }
        else if (programNode is ExpressionSyntax expression)
        {
            // For backward compatibility with single expressions
            body = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ParseName("System.Console"),
                            SyntaxFactory.IdentifierName("WriteLine")
                        ),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(expression)))
                    )
                )
            );
        }
        else
        {
            body = SyntaxFactory.Block();
        }

        // Create the Main method
        var mainMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Main")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithBody(body);

        // Create the Program class
        var classDeclaration = SyntaxFactory.ClassDeclaration("Program")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(mainMethod);

        // Create the namespace declaration
        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("GeneratedProgram"))
            .AddMembers(classDeclaration);

        // Create the compilation unit
        var compilationUnit = SyntaxFactory.CompilationUnit()
            .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
            .AddMembers(namespaceDeclaration);

        return compilationUnit.NormalizeWhitespace();
    }

    private List<StatementSyntax> ConvertBlockStatements(List<StatementSyntax> statements)
    {
        if (statements.Count == 0) return statements;

        var result = new List<StatementSyntax>();
        for (int i = 0; i < statements.Count; i++)
        {
            var statement = statements[i];
            
            // If it's the last statement and it's an expression statement, convert it to WriteLine
            if (i == statements.Count - 1 && statement is ExpressionStatementSyntax exprStmt)
            {
                result.Add(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ParseName("System.Console"),
                            SyntaxFactory.IdentifierName("WriteLine")
                        ),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(exprStmt.Expression)))
                    )
                ));
            }
            // Handle if statements with expression blocks
            else if (statement is IfStatementSyntax ifStmt)
            {
                result.Add(ConvertIfStatement(ifStmt));
            }
            else
            {
                result.Add(statement);
            }
        }
        return result;
    }

    private IfStatementSyntax ConvertIfStatement(IfStatementSyntax ifStmt)
    {
        var thenStmt = ConvertStatementToReturning(ifStmt.Statement);
        var elseStmt = ifStmt.Else?.Statement != null ? 
            SyntaxFactory.ElseClause(ConvertStatementToReturning(ifStmt.Else.Statement)) : 
            ifStmt.Else;
        
        return ifStmt.WithStatement(thenStmt).WithElse(elseStmt);
    }

    private StatementSyntax ConvertStatementToReturning(StatementSyntax statement)
    {
        if (statement is BlockSyntax block)
        {
            var statements = block.Statements.ToList();
            if (statements.Count > 0 && statements.Last() is ExpressionStatementSyntax lastExpr)
            {
                statements[statements.Count - 1] = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ParseName("System.Console"),
                            SyntaxFactory.IdentifierName("WriteLine")
                        ),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(lastExpr.Expression)))
                    )
                );
            }
            return SyntaxFactory.Block(statements);
        }
        return statement;
    }

    private ExpressionSyntax CreateDisplayExpression(ExpressionSyntax expression)
    {
        // For arrays, use string.Join to display nicely
        return SyntaxFactory.ConditionalExpression(
            // Condition: expression is object[]
            SyntaxFactory.BinaryExpression(
                SyntaxKind.IsExpression,
                SyntaxFactory.ParenthesizedExpression(expression),
                SyntaxFactory.ParseTypeName("object[]")
            ),
            // True: "[" + string.Join(", ", array) + "]"
            SyntaxFactory.BinaryExpression(
                SyntaxKind.AddExpression,
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.AddExpression,
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("[")),
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ParseName("System.String"),
                            SyntaxFactory.IdentifierName("Join")
                        ),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] {
                            SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(", "))),
                            SyntaxFactory.Argument(SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object[]"), expression))
                        }))
                    )
                ),
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("]"))
            ),
            // False: expression.ToString()
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    expression,
                    SyntaxFactory.IdentifierName("ToString")
                )
            )
        );
    }
}