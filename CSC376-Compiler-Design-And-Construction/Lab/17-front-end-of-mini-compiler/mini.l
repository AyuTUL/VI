%option noyywrap yylineno
%{
#include "mini.tab.h"
#include <string.h>
#include <stdlib.h>
%}
%%
"int"                   { return INT; }
"float"                 { return FLOAT; }
"print"                 { return PRINT; }
[0-9]+(\.[0-9]+)?       { yylval.str=strdup(yytext); return NUMBER; }
[a-zA-Z_][a-zA-Z0-9_]*  { yylval.str=strdup(yytext); return ID; }
"//"[^\n]*               ;
[ \t\r\n]+              ;
.                         { return yytext[0]; }
%%
