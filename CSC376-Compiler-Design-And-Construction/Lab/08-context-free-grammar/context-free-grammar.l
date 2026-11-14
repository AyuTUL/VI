/* Sample Input :
a+b*c
a+*b
*/

%option noyywrap
%{
#include "context-free-grammar.tab.h"
%}
%%
[a-zA-Z_][a-zA-Z0-9_]* { return ID; }
([0-9]+(\.[0-9]+)?|\.[0-9]+) { return NUMBER; }
[+*/()-] { return yytext[0]; }
[ \t\r]+ ;
\n { return '\n'; }
. { return INVALID; }
%%