grammar Simple;

program
    : (statement)* EOF
    ;

statement
    : variableDeclaration
    | assignment
    | functionDefinition
    | returnStatement
    | ifStatement
    | whileStatement
    | expression ';'
    | block
    ;

block
    : '{' (statement)* '}'
    ;

variableDeclaration
    : 'let' ID ('=' expression)? ';'
    ;

assignment
    : ID '=' expression ';'
    ;

functionDefinition
    : 'fn' ID '(' parameterList? ')' ('=>' expression ';' | block)
    ;

parameterList
    : ID (',' ID)*
    ;

returnStatement
    : 'return' expression? ';'
    ;

ifStatement
    : 'if' '(' expression ')' statement ('else' statement)?
    ;

whileStatement
    : 'while' '(' expression ')' statement
    ;

expression
    : primary
    | expression '(' argumentList? ')'                          // function call
    | expression ('*' | '/' | '%') expression                   // multiplication
    | expression ('+' | '-') expression                         // addition
    | expression ('<' | '>' | '<=' | '>=' | '==' | '!=') expression // comparison
    | expression ('&&' | '||') expression                       // logical
    | '!' expression                                             // negation
    | '(' expression ')'                                         // grouping
    ;

primary
    : INT
    | FLOAT
    | STRING
    | BOOL
    | ID
    | arrayLiteral
    | lambdaExpression
    ;

arrayLiteral
    : '[' (expression (',' expression)*)? ']'
    ;

lambdaExpression
    : '|' parameterList? '|' ('=>' expression | block)
    ;

argumentList
    : expression (',' expression)*
    ;

// Lexer rules
COMMENT : '//' ~[\r\n]* -> skip ;
MULTILINE_COMMENT : '/*' .*? '*/' -> skip ;

BOOL    : 'true' | 'false' ;
STRING  : '"' (~["\\\r\n] | '\\' .)* '"' ;
FLOAT   : [0-9]+ '.' [0-9]+ ;
INT     : [0-9]+ ;
ID      : [a-zA-Z_][a-zA-Z_0-9]* ;
WS      : [ \t\r\n]+ -> skip ;