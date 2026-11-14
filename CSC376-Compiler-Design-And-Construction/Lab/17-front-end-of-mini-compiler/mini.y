%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "symbol_table.h"
int yylex(void); void yyerror(const char*s); extern int yylineno;
int tempno=0, quad=0;
char current_type[16];
char *temp(void){char b[20];sprintf(b,"t%d",++tempno);return strdup(b);}
char *emitbin(char*a,const char*op,char*b){char*t=temp();printf("%3d: %s = %s %s %s\n",quad++,t,a,op,b);free(a);free(b);return t;}
void ensure_declared(const char*n){if(!st_type(n)){fprintf(stderr,"Semantic error at line %d: '%s' is not declared.\n",yylineno,n);exit(1);}}
%}
%union { char *str; }
%token INT FLOAT PRINT
%token <str> ID NUMBER
%type <str> expr term factor
%%
program : items { puts("\nCompilation front-end completed successfully."); st_print(); }
        ;
items : items item | /* empty */ ;
item : declaration ';'
     | assignment ';'
     | PRINT '(' expr ')' ';' { printf("%3d: print %s\n",quad++,$3); free($3); }
     ;
declaration : INT {strcpy(current_type,"int");} idlist
            | FLOAT {strcpy(current_type,"float");} idlist
            ;
idlist : ID { if(!st_insert($1,current_type)){fprintf(stderr,"Redeclaration of %s\n",$1);exit(1);} free($1); }
       | idlist ',' ID { if(!st_insert($3,current_type)){fprintf(stderr,"Redeclaration of %s\n",$3);exit(1);} free($3); }
       ;
assignment : ID '=' expr { ensure_declared($1); printf("%3d: %s = %s\n",quad++,$1,$3); free($1); free($3); }
           ;
expr : expr '+' term { $$=emitbin($1,"+",$3); }
     | expr '-' term { $$=emitbin($1,"-",$3); }
     | term          { $$=$1; }
     ;
term : term '*' factor { $$=emitbin($1,"*",$3); }
     | term '/' factor { $$=emitbin($1,"/",$3); }
     | factor          { $$=$1; }
     ;
factor : '(' expr ')' { $$=$2; }
       | NUMBER       { $$=$1; }
       | ID           { ensure_declared($1); $$=$1; }
       ;
%%
void yyerror(const char*s){ fprintf(stderr,"Syntax error at line %d: %s\n",yylineno,s); }
int main(void){ puts("Mini Compiler Front-End: lexical + syntax + semantic + TAC"); return yyparse(); }
