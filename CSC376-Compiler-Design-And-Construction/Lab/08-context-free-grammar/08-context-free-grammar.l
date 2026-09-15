%{
#include <stdio.h>
int yylex(void);
void yyerror(const char *message);
%}
%token ID NUMBER INVALID
%start input
%%
input : expression ;
expression : expression '+' term
           | expression '-' term
           | term ;
term : term '*' factor
     | term '/' factor
     | factor ;
factor : '(' expression ')'
       | ID
       | NUMBER ;
%%
void yyerror(const char *message) { fprintf(stderr, "Invalid expression: %s\n", message); }
int main(void) {
    int result = yyparse();
    if (result == 0) puts("Valid expression");
    return result == 0 ? 0 : 1;
}

FILE: lab8.l

%option noyywrap noinput nounput
%{
#include "lab8.tab.h"
%}
%%
[a-zA-Z_][a-zA-Z0-9_]*       { return ID; }
([0-9]+(\.[0-9]+)?|\.[0-9]+) { return NUMBER; }
[+*/()-]                     { return (unsigned char)yytext[0]; }
[ \t\r\n]+                  ;
.                            { return INVALID; }
%%