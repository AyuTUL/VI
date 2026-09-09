%option noyywrap noinput nounput
%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#define NS 256
#define DS 512
enum { LITERAL = 256 };
static int literal;
%}
%%
[a-zA-Z0-9] { literal = (unsigned char)yytext[0]; return LITERAL; }
[ \t\r\n]+ ;
.           { return (unsigned char)yytext[0]; }
%%
/* Thompson construction followed by epsilon-closure subset construction. */
typedef struct { int symbol, out1, out2; } State;
typedef struct { int start, end; } Fragment;
static State nfa[NS];
static int nn, look, alphabet[128], na;
static unsigned char subsets[DS][NS];
static int transition[DS][128], nd;
static void fail(const char *s) { fprintf(stderr, "Error: %s\n", s); exit(1); }
static int state(int symbol, int a, int b) {
    if (nn == NS) fail("NFA state limit exceeded");
    nfa[nn] = (State){symbol, a, b};
    return nn++;
}
static void advance(void) { look = yylex(); }
static Fragment expression(void);
static Fragment atom(void) {
    Fragment f;
    if (look == LITERAL || look == '#') {
        int c = look == '#' ? 0 : literal;
        int s = state(c, -1, -1), e = state(0, -1, -1);
        nfa[s].out1 = e;
        if (c) alphabet[c] = 1;
        advance();
        return (Fragment){s, e};
    }
    if (look == '(') {
        advance(); f = expression();
        if (look != ')') fail("missing closing parenthesis");
        advance(); return f;
    }
    fail("expected a literal, #, or parenthesized RE");
    return (Fragment){0, 0};
}
static Fragment repeat(void) {
    Fragment f = atom();
    while (look == '*') {
        int s = state(0, f.start, -1), e = state(0, -1, -1);
        nfa[s].out2 = e;
        nfa[f.end].out1 = f.start; nfa[f.end].out2 = e;
        f = (Fragment){s, e}; advance();
    }
    return f;
}
static Fragment concatenation(void) {
    Fragment f = repeat();
    while (look == LITERAL || look == '#' || look == '(') {
        Fragment g = repeat();
        nfa[f.end].out1 = g.start; f.end = g.end;
    }
    return f;
}
static Fragment expression(void) {
    Fragment f = concatenation();
    while (look == '|') {
        advance(); Fragment g = concatenation();
        int s = state(0, f.start, g.start), e = state(0, -1, -1);
        nfa[f.end].out1 = e; nfa[g.end].out1 = e;
        f = (Fragment){s, e};
    }
    return f;
}
static void closure(unsigned char set[NS]) {
    int changed;
    do {
        changed = 0;
        for (int i = 0; i < nn; ++i) if (set[i] && nfa[i].symbol == 0) {
            int out[2] = {nfa[i].out1, nfa[i].out2};
            for (int j = 0; j < 2; ++j)
                if (out[j] >= 0 && !set[out[j]]) { set[out[j]] = 1; changed = 1; }
        }
    } while (changed);
}
static int intern(unsigned char set[NS]) {
    for (int i = 0; i < nd; ++i)
        if (!memcmp(subsets[i], set, NS)) return i;
    if (nd == DS) fail("DFA state limit exceeded");
    memcpy(subsets[nd], set, NS); return nd++;
}
int main(void) {
    advance(); Fragment re = expression();
    if (look) fail("unexpected symbol after RE");
    int symbols[128];
    for (int c = 1; c < 128; ++c) if (alphabet[c]) symbols[na++] = c;
    unsigned char initial[NS] = {0}; initial[re.start] = 1;
    closure(initial); intern(initial);
    for (int d = 0; d < nd; ++d) for (int a = 0; a < na; ++a) {
        unsigned char next[NS] = {0};
        for (int s = 0; s < nn; ++s)
            if (subsets[d][s] && nfa[s].symbol == symbols[a] && nfa[s].out1 >= 0)
                next[nfa[s].out1] = 1;
        closure(next); transition[d][a] = intern(next);
    }
    printf("Start state: D0\nFinal states:");
    for (int d = 0; d < nd; ++d) if (subsets[d][re.end]) printf(" D%d", d);
    printf("\nState");
    for (int a = 0; a < na; ++a) printf("\t%c", symbols[a]);
    puts("\tNFA subset");
    for (int d = 0; d < nd; ++d) {
        printf("D%d", d);
        for (int a = 0; a < na; ++a) printf("\tD%d", transition[d][a]);
        printf("\t{"); int first = 1;
        for (int s = 0; s < nn; ++s) if (subsets[d][s]) {
            printf("%s%d", first ? "" : ",", s); first = 0;
        }
        puts("}");
    }
    return 0;
}