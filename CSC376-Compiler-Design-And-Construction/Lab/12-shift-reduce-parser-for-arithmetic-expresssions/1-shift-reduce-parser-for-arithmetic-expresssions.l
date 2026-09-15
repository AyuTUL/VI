%{
#include <stdio.h>
int yylex(void);
void yyerror(const char *message);
%}
%token ID NUMBER INVALID
%start input
%%
input : expression { puts("Reduce: input -> E"); } ;
expression : expression '+' term { puts("Reduce: E -> E + T"); }
           | expression '-' term { puts("Reduce: E -> E - T"); }
           | term                { puts("Reduce: E -> T"); } ;
term : term '*' factor { puts("Reduce: T -> T * F"); }
     | term '/' factor { puts("Reduce: T -> T / F"); }
     | factor          { puts("Reduce: T -> F"); } ;
factor : '(' expression ')' { puts("Reduce: F -> ( E )"); }
       | ID                 { puts("Reduce: F -> id"); }
       | NUMBER             { puts("Reduce: F -> number"); } ;
%%
void yyerror(const char *message) { fprintf(stderr, "Rejected: %s\n", message); }
int main(void) {
    /* The generated Yacc/Bison engine performs actual LR shifts/reductions.
       Compile with -DYYDEBUG=1 for its state/stack/shift trace on stderr. */
#if YYDEBUG
    yydebug = 1;
#endif
    int result = yyparse();
    if (!result) puts("Accepted (shift-reduce parser)");
    return result == 0 ? 0 : 1;
}
