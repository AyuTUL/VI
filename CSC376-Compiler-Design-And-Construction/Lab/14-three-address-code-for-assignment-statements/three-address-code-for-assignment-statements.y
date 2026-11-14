%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
int yylex(void); void yyerror(const char *s);
int tempNo=0, quadNo=0;
char *newtemp(void){ char b[20]; sprintf(b,"t%d",++tempNo); return strdup(b); }
char *emit_expr(const char *op, char *a, char *b){
    char *t=newtemp();
    printf("%-3d (%s, %s, %s, %s)\n",quadNo++,op,a,b,t);
    free(a); free(b); return t;
}
%}
%union { char *str; }
%token <str> ATOM
%token END_LINE
%type <str> expr term factor
%%
program : assignment END_LINE { YYACCEPT; } ;
assignment : ATOM '=' expr ';' {
    printf("%-3d (=, %s, -, %s)\n",quadNo++,$3,$1);
    free($1); free($3);
  }
  ;
expr : expr '+' term { $$=emit_expr("+",$1,$3); }
     | expr '-' term { $$=emit_expr("-",$1,$3); }
     | term          { $$=$1; }
     ;
term : term '*' factor { $$=emit_expr("*",$1,$3); }
     | term '/' factor { $$=emit_expr("/",$1,$3); }
     | factor          { $$=$1; }
     ;
factor : '(' expr ')' { $$=$2; }
       | ATOM         { $$=$1; }
       ;
%%
void yyerror(const char*s){ fprintf(stderr,"Syntax error: %s\n",s); }
int main(void){ puts("Enter an assignment ending with ';':"); puts("Quadruples: (op, arg1, arg2, result)"); return yyparse(); }
