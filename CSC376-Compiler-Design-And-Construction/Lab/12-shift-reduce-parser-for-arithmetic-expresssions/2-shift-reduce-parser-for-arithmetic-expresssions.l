%option noyywrap noinput nounput
%{
#include "lab12.tab.h"
%}
%%
[a-zA-Z_][a-zA-Z0-9_]*        { return ID; }
([0-9]+(\.[0-9]+)?|\.[0-9]+) { return NUMBER; }
[+*/()-]                      { return (unsigned char)yytext[0]; }
[ \t\r\n]+                   ;
.                             { return INVALID; }
%%