using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Antlr4Roslyn.Services;

public class AntlrToRoslynVisitor : SimpleBaseVisitor<SyntaxNode>
{
    private readonly Dictionary<string, TypeSyntax> _localVariables = new();

    public override SyntaxNode VisitProgram(SimpleParser.ProgramContext context)
    {
        var statements = new List<StatementSyntax>();
        
        foreach (var stmt in context.statement())
        {
            var result = Visit(stmt);
            if (result is StatementSyntax statement)
                statements.Add(statement);
            else if (result is ExpressionSyntax expr)
                statements.Add(SyntaxFactory.ExpressionStatement(expr));
        }
        
        return SyntaxFactory.Block(statements);
    }

    public override StatementSyntax VisitStatement(SimpleParser.StatementContext context)
    {
        var text = context.GetText();
        
        if (context.variableDeclaration() != null)
            return VisitVariableDeclaration(context.variableDeclaration());
        if (context.assignment() != null)
            return VisitAssignment(context.assignment());
        if (context.functionDefinition() != null)
            return VisitFunctionDefinition(context.functionDefinition());
        if (context.returnStatement() != null)
            return VisitReturnStatement(context.returnStatement());
        if (context.ifStatement() != null)
            return VisitIfStatement(context.ifStatement());
        if (context.whileStatement() != null)
            return VisitWhileStatement(context.whileStatement());
        if (context.expression() != null)
            return SyntaxFactory.ExpressionStatement(VisitExpression(context.expression()));
        if (context.block() != null)
            return VisitBlock(context.block());
            
        throw new NotSupportedException($"Unknown statement type: {text}");
    }

    public override BlockSyntax VisitBlock(SimpleParser.BlockContext context)
    {
        var statements = new List<StatementSyntax>();
        foreach (var stmt in context.statement())
        {
            statements.Add(VisitStatement(stmt));
        }
        return SyntaxFactory.Block(statements);
    }
    
    public override LocalDeclarationStatementSyntax VisitVariableDeclaration(SimpleParser.VariableDeclarationContext context)
    {
        var variableName = context.ID().GetText();
        var initializer = context.expression() != null
            ? SyntaxFactory.EqualsValueClause(VisitExpression(context.expression()))
            : null;

        var variable = SyntaxFactory.VariableDeclarator(variableName).WithInitializer(initializer);
        var declaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var")).AddVariables(variable);
        
        _localVariables[variableName] = SyntaxFactory.IdentifierName("var");
        return SyntaxFactory.LocalDeclarationStatement(declaration);
    }

    public override ExpressionStatementSyntax VisitAssignment(SimpleParser.AssignmentContext context)
    {
        var identifier = SyntaxFactory.IdentifierName(context.ID().GetText());
        var value = VisitExpression(context.expression());
        var assignment = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, identifier, value);
        return SyntaxFactory.ExpressionStatement(assignment);
    }

    public override LocalFunctionStatementSyntax VisitFunctionDefinition(SimpleParser.FunctionDefinitionContext context)
    {
        try
        {
            var name = context.ID()?.GetText() ?? "unnamed";
            var parameters = context.parameterList() != null ? CreateParameterList(context.parameterList()) : SyntaxFactory.SeparatedList<ParameterSyntax>();
            
            // Check if this is an expression-based function (fn name() => expr;)
            if (context.GetText().Contains("=>") && context.expression() != null)
            {
                var returnExpr = VisitExpression(context.expression());
                var returnStmt = SyntaxFactory.ReturnStatement(returnExpr);
                var body = SyntaxFactory.Block(returnStmt);
                return SyntaxFactory.LocalFunctionStatement(SyntaxFactory.IdentifierName("var"), name)
                    .WithParameterList(SyntaxFactory.ParameterList(parameters))
                    .WithBody(body);
            }
            else if (context.block() != null)
            {
                var body = VisitBlock(context.block());
                return SyntaxFactory.LocalFunctionStatement(SyntaxFactory.IdentifierName("var"), name)
                    .WithParameterList(SyntaxFactory.ParameterList(parameters))
                    .WithBody(body);
            }
            else
            {
                // Fallback to empty body
                var body = SyntaxFactory.Block();
                return SyntaxFactory.LocalFunctionStatement(SyntaxFactory.IdentifierName("var"), name)
                    .WithParameterList(SyntaxFactory.ParameterList(parameters))
                    .WithBody(body);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing function definition: {ex.Message}", ex);
        }
    }

    private SeparatedSyntaxList<ParameterSyntax> CreateParameterList(SimpleParser.ParameterListContext context)
    {
        var parameters = new List<ParameterSyntax>();
        foreach (var id in context.ID())
        {
            parameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(id.GetText())));
        }
        return SyntaxFactory.SeparatedList(parameters);
    }

    public override ReturnStatementSyntax VisitReturnStatement(SimpleParser.ReturnStatementContext context)
    {
        var expr = context.expression() != null ? VisitExpression(context.expression()) : null;
        return SyntaxFactory.ReturnStatement(expr);
    }

    public override IfStatementSyntax VisitIfStatement(SimpleParser.IfStatementContext context)
    {
        var condition = VisitExpression(context.expression());
        var thenStmt = VisitStatement(context.statement(0));
        var elseStmt = context.statement().Length > 1 ? SyntaxFactory.ElseClause(VisitStatement(context.statement(1))) : null;
        
        return SyntaxFactory.IfStatement(condition, thenStmt).WithElse(elseStmt);
    }

    public override WhileStatementSyntax VisitWhileStatement(SimpleParser.WhileStatementContext context)
    {
        var condition = VisitExpression(context.expression());
        var body = VisitStatement(context.statement());
        return SyntaxFactory.WhileStatement(condition, body);
    }

    public override ExpressionSyntax VisitExpression(SimpleParser.ExpressionContext context)
    {
        if (context.primary() != null)
            return VisitPrimary(context.primary());

        if (context.argumentList() != null)
        {
            var expr = VisitExpression(context.expression(0));
            var args = CreateArgumentList(context.argumentList());
            return SyntaxFactory.InvocationExpression(expr, SyntaxFactory.ArgumentList(args));
        }

        if (context.ChildCount == 3)
        {
            var left = VisitExpression(context.expression(0));
            var op = context.GetChild(1).GetText();
            var right = VisitExpression(context.expression(1));

            var kind = op switch
            {
                "+" => SyntaxKind.AddExpression,
                "-" => SyntaxKind.SubtractExpression,
                "*" => SyntaxKind.MultiplyExpression,
                "/" => SyntaxKind.DivideExpression,
                "%" => SyntaxKind.ModuloExpression,
                "<" => SyntaxKind.LessThanExpression,
                ">" => SyntaxKind.GreaterThanExpression,
                "<=" => SyntaxKind.LessThanOrEqualExpression,
                ">=" => SyntaxKind.GreaterThanOrEqualExpression,
                "==" => SyntaxKind.EqualsExpression,
                "!=" => SyntaxKind.NotEqualsExpression,
                "&&" => SyntaxKind.LogicalAndExpression,
                "||" => SyntaxKind.LogicalOrExpression,
                "(" when context.GetChild(2).GetText() == ")" => throw new InvalidOperationException("Should be handled by parentheses case"),
                _ => throw new NotSupportedException($"Unsupported operator: {op}")
            };

            if (op == "(" && context.GetChild(2).GetText() == ")")
                return left;

            return SyntaxFactory.BinaryExpression(kind, left, right);
        }

        if (context.ChildCount == 2 && context.GetChild(0).GetText() == "!")
        {
            var operand = VisitExpression(context.expression(0));
            return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, operand);
        }


        throw new NotSupportedException("Unsupported expression structure");
    }

    public override ExpressionSyntax VisitPrimary(SimpleParser.PrimaryContext context)
    {
        if (context.INT() != null)
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(int.Parse(context.INT().GetText())));
        
        if (context.FLOAT() != null)
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(double.Parse(context.FLOAT().GetText())));
        
        if (context.STRING() != null)
        {
            var text = context.STRING().GetText();
            var unquoted = text.Substring(1, text.Length - 2);
            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(unquoted));
        }
        
        if (context.BOOL() != null)
        {
            var value = context.BOOL().GetText() == "true";
            return SyntaxFactory.LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);
        }
        
        if (context.ID() != null)
            return SyntaxFactory.IdentifierName(context.ID().GetText());
        
        if (context.arrayLiteral() != null)
            return VisitArrayLiteral(context.arrayLiteral());
        
        if (context.lambdaExpression() != null)
            return VisitLambdaExpression(context.lambdaExpression());

        throw new NotSupportedException("Unsupported primary expression");
    }

    public override ExpressionSyntax VisitArrayLiteral(SimpleParser.ArrayLiteralContext context)
    {
        var elements = new List<ExpressionSyntax>();
        foreach (var expr in context.expression())
        {
            elements.Add(VisitExpression(expr));
        }
        
        var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, SyntaxFactory.SeparatedList(elements));
        return SyntaxFactory.ArrayCreationExpression(
            SyntaxFactory.ArrayType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                .WithRankSpecifiers(SyntaxFactory.SingletonList(
                    SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                        SyntaxFactory.OmittedArraySizeExpression())))))
            .WithInitializer(initializer);
    }

    public override ExpressionSyntax VisitLambdaExpression(SimpleParser.LambdaExpressionContext context)
    {
        var parameters = context.parameterList() != null ? CreateParameterList(context.parameterList()) : SyntaxFactory.SeparatedList<ParameterSyntax>();
        
        if (context.expression() != null)
        {
            var body = VisitExpression(context.expression());
            
            // Create a cast to Func<T, TResult> to help with type inference
            ExpressionSyntax lambda;
            if (parameters.Count == 1)
            {
                lambda = SyntaxFactory.SimpleLambdaExpression(parameters[0], body);
            }
            else if (parameters.Count == 0)
            {
                lambda = SyntaxFactory.ParenthesizedLambdaExpression(SyntaxFactory.ParameterList(), body);
            }
            else
            {
                lambda = SyntaxFactory.ParenthesizedLambdaExpression(SyntaxFactory.ParameterList(parameters), body);
            }
            
            // Cast to Func delegate type to help type inference
            var funcType = parameters.Count switch
            {
                0 => "System.Func<dynamic>",
                1 => "System.Func<dynamic, dynamic>",
                2 => "System.Func<dynamic, dynamic, dynamic>",
                _ => "System.Func<dynamic>"
            };
            
            return SyntaxFactory.CastExpression(
                SyntaxFactory.ParseTypeName(funcType),
                SyntaxFactory.ParenthesizedExpression(lambda)
            );
        }
        else
        {
            var body = VisitBlock(context.block());
            var lambda = SyntaxFactory.ParenthesizedLambdaExpression(SyntaxFactory.ParameterList(parameters), body);
            
            var funcType = parameters.Count switch
            {
                0 => "System.Action",
                1 => "System.Action<dynamic>",
                2 => "System.Action<dynamic, dynamic>",
                _ => "System.Action<dynamic>"
            };
            
            return SyntaxFactory.CastExpression(
                SyntaxFactory.ParseTypeName(funcType),
                SyntaxFactory.ParenthesizedExpression(lambda)
            );
        }
    }

    private SeparatedSyntaxList<ArgumentSyntax> CreateArgumentList(SimpleParser.ArgumentListContext context)
    {
        var arguments = new List<ArgumentSyntax>();
        foreach (var expr in context.expression())
        {
            arguments.Add(SyntaxFactory.Argument(VisitExpression(expr)));
        }
        return SyntaxFactory.SeparatedList(arguments);
    }
}