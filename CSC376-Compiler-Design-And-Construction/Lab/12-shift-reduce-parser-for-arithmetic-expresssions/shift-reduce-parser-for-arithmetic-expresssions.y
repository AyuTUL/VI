%{
#include <stdio.h>
int yylex(void);
void yyerror(const char *message);
%}
%token ID NUMBER INVALID END_LINE
%left '+' '-'
%left '*' '/'
%right UMINUS
%start input
%%
input : expression END_LINE { puts("Reduce: input -> E"); } ;
expression : expression '+' expression { puts("Reduce: E -> E + E"); }
           | expression '-' expression { puts("Reduce: E -> E - E"); }
           | expression '*' expression { puts("Reduce: E -> E * E"); }
           | expression '/' expression { puts("Reduce: E -> E / E"); }
           | '(' expression ')'        { puts("Reduce: E -> ( E )"); }
           | '-' expression %prec UMINUS { puts("Reduce: E -> - E"); }
           | ID                         { puts("Reduce: E -> id"); }
           | NUMBER                     { puts("Reduce: E -> number"); } ;
%%
void yyerror(const char *message) { fprintf(stderr, "Rejected: %s\n", message); }
int main(void) {
    fputs("Enter expression: ", stdout);
#if YYDEBUG
    yydebug = 1;
#endif
    int result = yyparse();
    if (!result) puts("Accepted (shift-reduce parser)");
    return result == 0 ? 0 : 1;
}