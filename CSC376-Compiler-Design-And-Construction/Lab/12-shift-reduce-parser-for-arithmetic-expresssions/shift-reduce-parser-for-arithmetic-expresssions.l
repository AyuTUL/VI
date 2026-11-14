/* Sample Input :
a+b*c
*/

%option noyywrap noinput nounput
%{
#include "shift-reduce-parser-for-arithmetic-expresssions.tab.h"
%}
%%
[a-zA-Z_][a-zA-Z0-9_]*        { return ID; }
([0-9]+(\.[0-9]+)?|\.[0-9]+) { return NUMBER; }
[+*/()-]                      { return (unsigned char)yytext[0]; }
\n                            { return END_LINE; }
[ \t\r]+                     ;
.                             { return INVALID; }
%%