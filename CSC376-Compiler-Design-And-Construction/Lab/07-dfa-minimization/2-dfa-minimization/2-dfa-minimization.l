%option noyywrap noinput nounput
%{
#include <stdio.h>
#include <stdlib.h>
#include <errno.h>
enum { INTEGER = 256, SYMBOL };
static long value;
static char symbol;
%}
%%
[0-9]+ { errno = 0; value = strtol(yytext, NULL, 10);
          if (errno) return '?'; return INTEGER; }
[a-zA-Z] { symbol = yytext[0]; return SYMBOL; }
[ \t\r\n]+ ;
. { return (unsigned char)yytext[0]; }
%%
#define MAX 128
#define ALPHA 52
static int delta[MAX][ALPHA], final[MAX], reachable[MAX];
static int different[MAX][MAX], group[MAX], representative[MAX];
static char alphabet[ALPHA];
static void fail(const char *s) { fprintf(stderr, "Error: %s\n", s); exit(1); }
static int number(int lo, int hi) {
    if (yylex() != INTEGER || value < lo || value > hi) fail("invalid integer or range");
    return (int)value;
}
int main(void) {
    int n = number(1, MAX), k = number(1, ALPHA);
    int start = number(0, n - 1), nf = number(0, n);
    for (int i = 0; i < nf; ++i) {
        int f = number(0, n - 1);
        if (final[f]) fail("duplicate final state");
        final[f] = 1;
    }
    for (int a = 0; a < k; ++a) {
        if (yylex() != SYMBOL) fail("alphabet symbols must be single letters");
        alphabet[a] = symbol;
        for (int b = 0; b < a; ++b)
            if (alphabet[b] == symbol) fail("duplicate alphabet symbol");
    }
    for (int s = 0; s < n; ++s)
        for (int a = 0; a < k; ++a) delta[s][a] = number(0, n - 1);
    if (yylex()) fail("extra input after transition table");
    int queue[MAX], head = 0, tail = 0;
    queue[tail++] = start; reachable[start] = 1;
    while (head < tail) {
        int s = queue[head++];
        for (int a = 0; a < k; ++a) {
            int t = delta[s][a];
            if (!reachable[t]) { reachable[t] = 1; queue[tail++] = t; }
        }
    }
    for (int i = 0; i < n; ++i) for (int j = 0; j < n; ++j)
        different[i][j] = final[i] != final[j];
    int changed;
    do {
        changed = 0;
        for (int i = 0; i < n; ++i) if (reachable[i])
            for (int j = 0; j < i; ++j) if (reachable[j] && !different[i][j])
                for (int a = 0; a < k; ++a)
                    if (different[delta[i][a]][delta[j][a]]) {
                        different[i][j] = different[j][i] = 1;
                        changed = 1; break;
                    }
    } while (changed);
    int ng = 0;
    for (int s = 0; s < n; ++s) {
        group[s] = -1;
        if (!reachable[s]) continue;
        for (int g = 0; g < ng; ++g) if (!different[s][representative[g]]) {
            group[s] = g; break;
        }
        if (group[s] < 0) { representative[ng] = s; group[s] = ng++; }
    }
    printf("Unreachable states:");
    int removed = 0;
    for (int s = 0; s < n; ++s) if (!reachable[s]) { printf(" %d", s); removed = 1; }
    if (!removed) printf(" none");
    printf("\nMinimized states: %d\nStart state: M%d\nFinal states:", ng, group[start]);
    for (int g = 0; g < ng; ++g) if (final[representative[g]]) printf(" M%d", g);
    puts("");
    for (int g = 0; g < ng; ++g) {
        printf("M%d = {", g); int first = 1;
        for (int s = 0; s < n; ++s) if (group[s] == g) {
            printf("%s%d", first ? "" : ",", s); first = 0;
        }
        puts("}");
    }
    printf("State");
    for (int a = 0; a < k; ++a) printf("\t%c", alphabet[a]);
    puts("");
    for (int g = 0; g < ng; ++g) {
        printf("M%d", g);
        for (int a = 0; a < k; ++a) printf("\tM%d", group[delta[representative[g]][a]]);
        puts("");
    }
    return 0;
}