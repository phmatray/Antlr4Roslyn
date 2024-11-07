grammar Simple;

program : (statement)* EOF;

statement : expression ';' ;

expression
    : INT
    | ID
    | expression ('+' | '-' | '*' | '/') expression
    | '(' expression ')'
    ;

ID  : [a-zA-Z_][a-zA-Z_0-9]* ;
INT : [0-9]+ ;
WS  : [ \t\r\n]+ -> skip ;