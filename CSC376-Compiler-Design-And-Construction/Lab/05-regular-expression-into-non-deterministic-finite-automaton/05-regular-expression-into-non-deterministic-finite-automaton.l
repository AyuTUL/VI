%{
#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>

#define MAX 100

typedef struct {
    int from, to;
    char symbol;
} Transition;

typedef struct {
    int start, end;
} Fragment;

Transition t[500];
int tc = 0, sc = 0;

int newState() {
    return sc++;
}

void add(int from, int to, char symbol) {
    t[tc++] = (Transition){from, to, symbol};
}

Fragment symbol(char c) {
    Fragment f = {newState(), newState()};
    add(f.start, f.end, c);
    return f;
}

Fragment concat(Fragment a, Fragment b) {
    add(a.end, b.start, 'e');
    return (Fragment){a.start, b.end};
}

Fragment uni(Fragment a, Fragment b) {
    Fragment f = {newState(), newState()};

    add(f.start, a.start, 'e');
    add(f.start, b.start, 'e');
    add(a.end, f.end, 'e');
    add(b.end, f.end, 'e');

    return f;
}

Fragment star(Fragment a) {
    Fragment f = {newState(), newState()};

    add(f.start, a.start, 'e');
    add(f.start, f.end, 'e');
    add(a.end, a.start, 'e');
    add(a.end, f.end, 'e');

    return f;
}

int prec(char c) {
    if (c == '*') return 3;
    if (c == '.') return 2;
    if (c == '|') return 1;
    return 0;
}

int operand(char c) {
    return isalnum((unsigned char)c);
}

void postfix(char *in, char *out) {
    char stack[MAX];
    int top = -1, j = 0;

    for (int i = 0; in[i]; i++) {
        char c = in[i];

        if (c == ' ')
            continue;

        if (operand(c))
            out[j++] = c;

        else if (c == '(')
            stack[++top] = c;

        else if (c == ')') {
            while (top >= 0 && stack[top] != '(')
                out[j++] = stack[top--];
            if (top >= 0) top--;
        }

        else {
            while (top >= 0 &&
                   stack[top] != '(' &&
                   prec(stack[top]) >= prec(c))
                out[j++] = stack[top--];

            stack[++top] = c;
        }
    }

    while (top >= 0)
        out[j++] = stack[top--];

    out[j] = '\0';
}

Fragment build(char *p) {
    Fragment s[MAX];
    int top = -1;

    for (int i = 0; p[i]; i++) {
        char c = p[i];

        if (operand(c))
            s[++top] = symbol(c);

        else if (c == '.') {
            Fragment b = s[top--], a = s[top--];
            s[++top] = concat(a, b);
        }

        else if (c == '|') {
            Fragment b = s[top--], a = s[top--];
            s[++top] = uni(a, b);
        }

        else if (c == '*')
            s[top] = star(s[top]);
    }

    return s[top];
}

void printNFA(Fragment f) {
    printf("\nNFA Transition Table :\n");
    printf("-----------------------------\n");
    printf("From\tSymbol\tTo\n");
    printf("-----------------------------\n");

    for (int i = 0; i < tc; i++)
        printf("q%d\t%c\tq%d\n", t[i].from, t[i].symbol, t[i].to);

    printf("-----------------------------\n");
    printf("Start State : q%d\n", f.start);
    printf("Final State : q%d\n", f.end);
}

%}

%%

[^\n]+ {
    char p[MAX];

    printf("\nGiven Regular Expression: %s\n", yytext);

    postfix(yytext, p);
    printf("Postfix Expression: %s\n", p);

    Fragment f = build(p);
    printNFA(f);

    sc = tc = 0;
}

\n ;

%%

int main() {
    printf("Enter Regular Expression:\n");
    yylex();
    return 0;
}

int yywrap() {
    return 1;
}