/* Sample Input :
4
1
a
0
1
3
2
0 a 1
2 a 3
1
0 2
*/

%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define MAX_STATES 20
#define MAX_SYMBOLS 10
#define MAX_DFA_STATES 100

int nfaStates, nfaSymbols;
char symbols[MAX_SYMBOLS];

int transition[MAX_STATES][MAX_SYMBOLS][MAX_STATES];
int epsilon[MAX_STATES][MAX_STATES];

int startState;
int finalStates[MAX_STATES];

int dfa[MAX_DFA_STATES][MAX_SYMBOLS];
int dfaSets[MAX_DFA_STATES][MAX_STATES];
int dfaCount;

void epsilonClosure(int state, int set[]) {
    set[state] = 1;
    for (int i = 0; i < nfaStates; i++)
        if (epsilon[state][i] && !set[i])
            epsilonClosure(i, set);
}

void getNextSet(int set[], int symbol, int result[]) {
    int move[MAX_STATES] = {0};
    for (int i = 0; i < nfaStates; i++)
        if (set[i])
            for (int j = 0; j < nfaStates; j++)
                if (transition[i][symbol][j])
                    move[j] = 1;
    for (int i = 0; i < nfaStates; i++)
        if (move[i])
            epsilonClosure(i, result);
}

int sameSet(int a[], int b[]) {
    return !memcmp(a, b, sizeof(int) * nfaStates);
}

int findSet(int set[]) {
    for (int i = 0; i < dfaCount; i++)
        if (sameSet(dfaSets[i], set))
            return i;
    return -1;
}

int addDFAState(int set[]) {
    int index = findSet(set);
    if (index != -1)
        return index;
    memcpy(dfaSets[dfaCount], set, sizeof(int) * nfaStates);
    return dfaCount++;
}

void printSet(int set[]) {
    printf("{");
    int first = 1;
    for (int i = 0; i < nfaStates; i++)
        if (set[i]) {
            if (!first)
                printf(",");
            printf("q%d", i);
            first = 0;
        }
    printf("}");
}

void constructDFA(void) {
    int start[MAX_STATES] = {0};
    epsilonClosure(startState, start);
    addDFAState(start);
    for (int d = 0; d < dfaCount; d++)
        for (int s = 0; s < nfaSymbols; s++) {
            int result[MAX_STATES] = {0};
            getNextSet(dfaSets[d], s, result);
            dfa[d][s] = addDFAState(result);
        }
}

void printDFA(void) {
    printf("\nDFA Transition Table\n----------------------------------------------\n");
    printf("%-16s", "State");
    for (int i = 0; i < nfaSymbols; i++)
        printf("%-12c", symbols[i]);
    printf("\n----------------------------------------------\n");
    for (int i = 0; i < dfaCount; i++) {
        char state[50] = "";
        sprintf(state, "D%d ", i);
        int first = 1;
        strcat(state, "{");
        for (int q = 0; q < nfaStates; q++)
            if (dfaSets[i][q]) {
                char temp[10];
                if (!first)
                    strcat(state, ",");
                sprintf(temp, "q%d", q);
                strcat(state, temp);
                first = 0;
            }
        strcat(state, "}");
        printf("%-16s", state);
        for (int s = 0; s < nfaSymbols; s++) {
            char dest[10];
            sprintf(dest, "D%d", dfa[i][s]);
            printf("%-12s", dest);
        }
        printf("\n");
    }
    printf("----------------------------------------------\n\nDFA Final States: ");
    for (int i = 0; i < dfaCount; i++)
        for (int q = 0; q < nfaStates; q++)
            if (dfaSets[i][q] && finalStates[q]) {
                printf("D%d ", i);
                break;
            }
    printf("\n");
}

%}

%%

[^\n]+ {
    nfaStates = atoi(yytext);
    printf("\nEnter number of input symbols: ");
    scanf("%d", &nfaSymbols);
    printf("Enter input symbols: ");
    for (int i = 0; i < nfaSymbols; i++)
        scanf(" %c", &symbols[i]);
    printf("\nEnter start state: ");
    scanf("%d", &startState);
    printf("Enter number of final states: ");
    int nf;
    scanf("%d", &nf);
    printf("Enter final states: ");
    for (int i = 0; i < nf; i++) {
        int f;
        scanf("%d", &f);
        finalStates[f] = 1;
    }
    int n;
    printf("\nEnter number of normal transitions: ");
    scanf("%d", &n);
    printf("Enter transitions as:\nfrom symbol to\n");
    for (int i = 0; i < n; i++) {
        int from, to;
        char symbol;
        scanf("%d %c %d", &from, &symbol, &to);
        for (int s = 0; s < nfaSymbols; s++)
            if (symbols[s] == symbol) {
                transition[from][s][to] = 1;
                break;
            }
    }
    printf("\nEnter number of epsilon transitions: ");
    scanf("%d", &n);
    printf("Enter epsilon transitions as:\nfrom to\n");
    for (int i = 0; i < n; i++) {
        int from, to;
        scanf("%d %d", &from, &to);
        epsilon[from][to] = 1;
    }

    constructDFA();
    printDFA();
}

\n {
}

%%

int main(void) { printf("Enter number of NFA states: "); yylex(); return 0; }
int yywrap(void) { return 1; }