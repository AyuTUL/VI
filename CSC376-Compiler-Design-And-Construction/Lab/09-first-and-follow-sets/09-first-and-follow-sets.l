%option noyywrap noinput nounput
%{
#ifndef GRAMMAR_CORE_H
#define GRAMMAR_CORE_H
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define MAX_PROD 256
#define MAX_RHS 64
#define ASCII 128
enum { ARROW = 256, BAD_TOKEN };
int yylex(void);
typedef struct { int lhs; char rhs[MAX_RHS]; } Production;
static Production prod[MAX_PROD];
static int np, start_symbol = -1, defined[26], used[26], terminals[ASCII];
/* Index zero in FIRST is epsilon; FOLLOW never contains epsilon. */
static unsigned char first_set[26][ASCII], follow_set[26][ASCII];
static void fail(const char *s) { fprintf(stderr, "Error: %s\n", s); exit(1); }
static int nonterminal(int c) { return c >= 'A' && c <= 'Z'; }
static void add_production(int lhs, const char *rhs) {
    if (np == MAX_PROD) fail("too many productions");
    for (int p = 0; p < np; ++p)
        if (prod[p].lhs == lhs && !strcmp(prod[p].rhs, rhs)) return;
    prod[np].lhs = lhs; strcpy(prod[np++].rhs, rhs);
}
static void read_grammar(void) {
    int t = yylex();
    while (t) {
        if (t == '\n') { t = yylex(); continue; }
        if (!nonterminal(t)) fail("LHS must be one uppercase letter");
        int lhs = t - 'A'; defined[lhs] = 1;
        if (start_symbol < 0) start_symbol = lhs;
        if (yylex() != ARROW) fail("expected -> after LHS");
        t = yylex();
        for (;;) {
            char rhs[MAX_RHS]; int len = 0, epsilon = 0;
            while (t && t != '\n' && t != '|') {
                if (t == '#') {
                    if (len || epsilon) fail("# must be the entire alternative");
                    epsilon = 1;
                } else {
                    if (epsilon || t < 33 || t >= ASCII || t == '$')
                        fail("invalid RHS symbol or mixed epsilon");
                    if (len == MAX_RHS - 1) fail("RHS too long");
                    rhs[len++] = (char)t;
                    if (nonterminal(t)) used[t - 'A'] = 1;
                    else terminals[t] = 1;
                }
                t = yylex();
            }
            if (!len && !epsilon) fail("empty alternative; use # for epsilon");
            rhs[len] = '\0'; add_production(lhs, rhs);
            if (t != '|') break;
            t = yylex();
        }
        if (t == '\n') t = yylex();
    }
    if (!np) fail("empty grammar");
    for (int n = 0; n < 26; ++n)
        if (used[n] && !defined[n]) fail("an RHS nonterminal has no production");
}
static int unite(unsigned char *dst, const unsigned char *src, int include_epsilon) {
    int changed = 0;
    for (int c = include_epsilon ? 0 : 1; c < ASCII; ++c)
        if (src[c] && !dst[c]) { dst[c] = 1; changed = 1; }
    return changed;
}
static void first_of_string(const char *s, unsigned char result[ASCII]) {
    memset(result, 0, ASCII);
    for (int i = 0; s[i]; ++i) {
        int c = (unsigned char)s[i];
        if (!nonterminal(c)) { result[c] = 1; return; }
        unite(result, first_set[c - 'A'], 0);
        if (!first_set[c - 'A'][0]) return;
    }
    result[0] = 1;
}
static void compute_sets(void) {
    int changed;
    do {
        changed = 0;
        for (int p = 0; p < np; ++p) {
            unsigned char s[ASCII]; first_of_string(prod[p].rhs, s);
            changed |= unite(first_set[prod[p].lhs], s, 1);
        }
    } while (changed);
    follow_set[start_symbol]['$'] = 1;
    do {
        changed = 0;
        for (int p = 0; p < np; ++p)
            for (int i = 0; prod[p].rhs[i]; ++i) {
                int c = (unsigned char)prod[p].rhs[i];
                if (!nonterminal(c)) continue;
                unsigned char suffix[ASCII]; first_of_string(prod[p].rhs + i + 1, suffix);
                changed |= unite(follow_set[c - 'A'], suffix, 0);
                if (suffix[0]) changed |= unite(follow_set[c - 'A'], follow_set[prod[p].lhs], 0);
            }
    } while (changed);
}
static void print_set(const unsigned char *set) {
    printf("{ ");
    for (int c = 0; c < ASCII; ++c) if (set[c]) printf("%c ", c ? c : '#');
    puts("}");
}
static void print_production(int p) {
    printf("%c->%s", 'A' + prod[p].lhs, prod[p].rhs[0] ? prod[p].rhs : "#");
}
static void print_sets(void) {
    printf("Start symbol: %c\n", 'A' + start_symbol);
    for (int n = 0; n < 26; ++n) if (defined[n]) {
        printf("FIRST(%c) = ", 'A' + n); print_set(first_set[n]);
        printf("FOLLOW(%c) = ", 'A' + n); print_set(follow_set[n]);
    }
}
int main(void) {
    read_grammar(); compute_sets(); print_sets();
    /* Also list the exact productions analyzed. */
    puts("Productions:");
    for (int p = 0; p < np; ++p) { print_production(p); puts(""); }
    return 0;
}
#endif
%}
%%
"->"         { return ARROW; }
[ \t\r]+    ;
\n           { return '\n'; }
[\x21-\x7E] { return (unsigned char)yytext[0]; }
.            { return BAD_TOKEN; }
%%
