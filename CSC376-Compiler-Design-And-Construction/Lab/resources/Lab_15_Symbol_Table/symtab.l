%option noyywrap yylineno
%{
#include <stdio.h>
#include <string.h>
#include <stdlib.h>

typedef struct S { char name[64]; int first_line; struct S *next; } S;
#define HSIZE 31
S *tab[HSIZE];
unsigned h(const char*s){unsigned x=5381;while(*s)x=((x<<5)+x)+(unsigned char)*s++;return x%HSIZE;}
void add(const char*s){unsigned k=h(s);for(S*p=tab[k];p;p=p->next)if(!strcmp(p->name,s))return;S*n=malloc(sizeof(*n));strcpy(n->name,s);n->first_line=yylineno;n->next=tab[k];tab[k]=n;}
void printtab(void){puts("Identifier\tFirst line");for(int i=0;i<HSIZE;i++)for(S*p=tab[i];p;p=p->next)printf("%-16s%d\n",p->name,p->first_line);}
int keyword(const char*s){const char*k[]={"int","float","char","double","if","else","while","for","return","void",NULL};for(int i=0;k[i];i++)if(!strcmp(k[i],s))return 1;return 0;}
%}
%%
[a-zA-Z_][a-zA-Z0-9_]* { if(!keyword(yytext)) add(yytext); }
\n                        ;
.                         ;
%%
int main(void){ yylex(); printtab(); return 0; }
