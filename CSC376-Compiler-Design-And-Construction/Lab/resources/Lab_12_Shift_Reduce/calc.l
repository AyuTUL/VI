%option noyywrap
%{
#include "calc.tab.h"
#include <stdlib.h>
%}
%%
[0-9]+(\.[0-9]+)?   { yylval.num = atof(yytext); return NUMBER; }
[a-zA-Z_][a-zA-Z0-9_]* { return ID; }
[ \t\r\n]+          ;
.                     { return yytext[0]; }
%%
