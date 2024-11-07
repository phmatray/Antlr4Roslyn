using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Antlr4Roslyn.Services;

// Visiteur ANTLR pour générer les nœuds Roslyn
public class AntlrToRoslynVisitor : SimpleBaseVisitor<ExpressionSyntax>
{
    public override ExpressionSyntax VisitProgram(SimpleParser.ProgramContext context)
    {
        if (context.statement() != null && context.statement().Length > 0)
        {
            return Visit(context.statement(0).expression());
        }
        
        throw new NotSupportedException("Empty program structure.");
    }

    public override ExpressionSyntax VisitExpression(SimpleParser.ExpressionContext context)
    {
        if (context.ChildCount == 1)
        {
            if (context.INT() != null)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(int.Parse(context.INT().GetText())));
            }

            if (context.ID() != null)
            {
                return SyntaxFactory.IdentifierName(context.ID().GetText());
            }
        }
        else if (context.ChildCount == 3)
        {
            var left = Visit(context.GetChild(0));
            var op = context.GetChild(1).GetText();
            var right = Visit(context.GetChild(2));

            SyntaxKind kind = op switch
            {
                "+" => SyntaxKind.AddExpression,
                "-" => SyntaxKind.SubtractExpression,
                "*" => SyntaxKind.MultiplyExpression,
                "/" => SyntaxKind.DivideExpression,
                _ => throw new NotSupportedException($"Unsupported operator: {op}")
            };

            return SyntaxFactory.BinaryExpression(kind, left, right);
        }
        else if (context.ChildCount == 3 && context.GetChild(0).GetText() == "(")
        {
            return Visit(context.GetChild(1)); // Expression in parentheses
        }

        throw new NotSupportedException("Unsupported expression structure.");
    }
}