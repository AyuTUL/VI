%option noyywrap
%{
#include "backpatch.tab.h"
#include <string.h>
#include <stdlib.h>
%}
%%
"if"                    { return IF; }
"else"                  { return ELSE; }
"while"                 { return WHILE; }
"<="|">="|"=="|"!="|"<"|">" { yylval.str=strdup(yytext); return RELOP; }
[0-9]+(\.[0-9]+)?       { yylval.str=strdup(yytext); return ATOM; }
[a-zA-Z_][a-zA-Z0-9_]*  { yylval.str=strdup(yytext); return ID; }
[ \t\r\n]+              ;
.                         { return yytext[0]; }
%%
