%option noyywrap
%{
#include "tac.tab.h"
#include <string.h>
#include <stdlib.h>
%}
%%
[0-9]+(\.[0-9]+)?       { yylval.str = strdup(yytext); return ATOM; }
[a-zA-Z_][a-zA-Z0-9_]*  { yylval.str = strdup(yytext); return ATOM; }
[ \t\r\n]+              ;
.                         { return yytext[0]; }
%%
