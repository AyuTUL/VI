/* Sample Input :
a+b*c
*/

%option noyywrap
%{
#include <stdio.h>
#include <stdlib.h>
enum { ID = 256, NUMBER, INVALID };
enum { END_LINE = 259 };
%}
%%
[a-zA-Z_][a-zA-Z0-9_]*        { return ID; }
([0-9]+(\.[0-9]+)?|\.[0-9]+) { return NUMBER; }
[+*/()-]                      { return (unsigned char)yytext[0]; }
[ \t\r]+                     ;
\n                            { return END_LINE; }
.                             { return INVALID; }
%%
static int look, depth;
static void expression(void);
static void fail(const char *s) { fprintf(stderr, "Invalid expression: %s\n", s); exit(1); }
static void advance(void) { look = yylex(); }
static void factor(void) {
    if (look == ID || look == NUMBER) { advance(); return; }
    if (look == '(') {
        if (++depth > 256) fail("parenthesis nesting exceeds 256");
        advance(); expression();
        if (look != ')') fail("expected closing parenthesis");
        --depth; advance(); return;
    }
    fail("expected identifier, number, or opening parenthesis");
}
static void term(void) {
    factor();
    while (look == '*' || look == '/') { advance(); factor(); }
}
static void expression(void) {
    term();
    while (look == '+' || look == '-') { advance(); term(); }
}
int main(void) {
    fputs("Enter arithmetic expression: ", stdout);
    advance(); expression();
    if (look != END_LINE && look != 0) fail("unexpected trailing token");
    puts("Valid expression (recursive descent)");
    return 0;
}