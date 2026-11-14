%option noyywrap
%{
#include "ast.tab.h"
#include <string.h>
#include <stdlib.h>
%}
%%
[0-9]+(\.[0-9]+)?       { yylval.str = strdup(yytext); return VALUE; }
[a-zA-Z_][a-zA-Z0-9_]*  { yylval.str = strdup(yytext); return VALUE; }
[ \t\r\n]+              ;
.                         { return yytext[0]; }
%%
