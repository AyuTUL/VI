%{
#include <stdio.h>
%}
%%
[a-zA-Z]+   { printf("Word : %s\n", yytext); }
[0-9]+      { printf("Number : %s\n", yytext); }
[ \t\n]     { /* ignore whitespace */ }
.           { printf("Other : %s\n", yytext); }
%%
int yywrap()
{
    return 1;
}
int main()
{
    printf("Enter text (Ctrl+Z then Enter to end):\n");
    yylex();
    return 0;
}