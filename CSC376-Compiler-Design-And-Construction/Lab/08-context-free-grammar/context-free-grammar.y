%{
#include <stdio.h>
int yylex(void);
void yyerror(const char *);
%}
%token ID NUMBER INVALID
%left '+' '-'
%left '*' '/'
%start input

%%
input : expression '\n' ;
expression : expression '+' expression
           | expression '-' expression
           | expression '*' expression
           | expression '/' expression
           | '(' expression ')'
           | ID
           | NUMBER ;

%%
void yyerror(const char *message) { fprintf(stderr, "Invalid expression: %s\n", message); }
int main(void) {
    fputs("Enter expression: ", stdout);
    int result = yyparse();
    if (result == 0) puts("Valid expression");
    return result == 0 ? 0 : 1;
}