%option noyywrap
%{
#include "syntax-tree-for-arithmetric-expressions-old.tab.h"
#include <string.h>
#include <stdlib.h>
static char *copy_text(const char *text) {
    size_t size = strlen(text) + 1;
    return memcpy(malloc(size), text, size);
}
%}
%%
[0-9]+(\.[0-9]+)?       { yylval.str = copy_text(yytext); return VALUE; }
[a-zA-Z_][a-zA-Z0-9_]*  { yylval.str = copy_text(yytext); return VALUE; }
[ \t\r]+                ;
\n                      { return END_LINE; }
.                       { return yytext[0]; }
%%
