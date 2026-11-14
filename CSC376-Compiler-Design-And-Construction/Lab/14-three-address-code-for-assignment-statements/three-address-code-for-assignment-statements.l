/* Sample Input :
x = a + b * c;
y = (x - 2) / d;
*/

%option noyywrap
%{
#include "three-address-code-for-assignment-statements.tab.h"
#include <string.h>
#include <stdlib.h>
%}
%%
[0-9]+(\.[0-9]+)?       { yylval.str = strdup(yytext); return ATOM; }
[a-zA-Z_][a-zA-Z0-9_]*  { yylval.str = strdup(yytext); return ATOM; }
[ \t\r]+                ;
\n                      { return END_LINE; }
.                         { return yytext[0]; }
%%
