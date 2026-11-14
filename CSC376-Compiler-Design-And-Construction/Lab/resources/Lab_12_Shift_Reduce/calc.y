%{
#include <stdio.h>
#include <stdlib.h>
int yylex(void);
void yyerror(const char *s);
%}
%union { double num; }
%token <num> NUMBER
%token ID
%left '+' '-'
%left '*' '/'
%right UMINUS
%%
input : expr { printf("Accepted: syntactically valid expression.\n"); }
      ;
expr  : expr '+' expr
      | expr '-' expr
      | expr '*' expr
      | expr '/' expr
      | '(' expr ')'
      | '-' expr %prec UMINUS
      | NUMBER
      | ID
      ;
%%
void yyerror(const char *s){ fprintf(stderr,"Syntax error: %s\n",s); }
int main(void){ printf("Enter expression: "); return yyparse(); }
