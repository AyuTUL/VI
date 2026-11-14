#include <stdio.h>
#include <string.h>
#include <ctype.h>

#define MAX_P 50
#define MAX_RHS 100
#define MAX_SET 100

char lhs[MAX_P];
char rhs[MAX_P][MAX_RHS];
int n;
char nonterminals[MAX_P];
int nt_count = 0;
char firstSet[26][MAX_SET];
char followSet[26][MAX_SET];
int firstDone[26];
int followVisiting[26];

int nt_index(char c) { return c - 'A'; }
int is_nonterminal(char c) { return c >= 'A' && c <= 'Z'; }

void add_symbol(char *set, char c) {
    if (!strchr(set, c)) {
        int len = (int)strlen(set);
        set[len] = c;
        set[len + 1] = '\0';
    }
}

void add_set_except_epsilon(char *dest, const char *src) {
    for (int i = 0; src[i]; ++i)
        if (src[i] != '#') add_symbol(dest, src[i]);
}

int has_epsilon(const char *set) { return strchr(set, '#') != NULL; }

void compute_first(char X);

void first_of_string(const char *s, char *out) {
    out[0] = '\0';
    if (s[0] == '\0' || (s[0] == '#' && s[1] == '\0')) {
        add_symbol(out, '#');
        return;
    }

    int all_nullable = 1;
    for (int i = 0; s[i]; ++i) {
        char sym = s[i];
        if (!is_nonterminal(sym)) {
            add_symbol(out, sym);
            all_nullable = 0;
            break;
        }
        compute_first(sym);
        add_set_except_epsilon(out, firstSet[nt_index(sym)]);
        if (!has_epsilon(firstSet[nt_index(sym)])) {
            all_nullable = 0;
            break;
        }
    }
    if (all_nullable) add_symbol(out, '#');
}

void compute_first(char X) {
    int idx = nt_index(X);
    if (firstDone[idx]) return;
    /* A simple cycle guard. FIRST grows monotonically as productions are revisited. */
    firstDone[idx] = 1;

    for (int p = 0; p < n; ++p) {
        if (lhs[p] != X) continue;
        char temp[MAX_SET] = "";
        first_of_string(rhs[p], temp);
        for (int i = 0; temp[i]; ++i) add_symbol(firstSet[idx], temp[i]);
    }
}

void recompute_first_fixed_point(void) {
    int changed;
    do {
        char old[26][MAX_SET];
        memcpy(old, firstSet, sizeof(old));
        memset(firstDone, 0, sizeof(firstDone));
        for (int i = 0; i < nt_count; ++i) compute_first(nonterminals[i]);
        changed = memcmp(old, firstSet, sizeof(old)) != 0;
    } while (changed);
}

void compute_follow_fixed_point(char start) {
    add_symbol(followSet[nt_index(start)], '$');
    int changed;
    do {
        char old[26][MAX_SET];
        memcpy(old, followSet, sizeof(old));

        for (int p = 0; p < n; ++p) {
            int L = (int)strlen(rhs[p]);
            for (int i = 0; i < L; ++i) {
                char B = rhs[p][i];
                if (!is_nonterminal(B)) continue;

                char beta[MAX_RHS];
                strcpy(beta, rhs[p] + i + 1);
                char firstBeta[MAX_SET] = "";
                first_of_string(beta, firstBeta);
                add_set_except_epsilon(followSet[nt_index(B)], firstBeta);

                if (beta[0] == '\0' || has_epsilon(firstBeta)) {
                    for (int k = 0; followSet[nt_index(lhs[p])][k]; ++k)
                        add_symbol(followSet[nt_index(B)], followSet[nt_index(lhs[p])][k]);
                }
            }
        }
        changed = memcmp(old, followSet, sizeof(old)) != 0;
    } while (changed);
}

void register_nt(char c) {
    if (!strchr(nonterminals, c)) {
        nonterminals[nt_count++] = c;
        nonterminals[nt_count] = '\0';
    }
}

int main(void) {
    printf("Enter number of productions (one alternative per production): ");
    if (scanf("%d", &n) != 1 || n <= 0 || n > MAX_P) return 1;

    printf("Enter productions in the form E=TR. Use # for epsilon.\n");
    for (int i = 0; i < n; ++i) {
        char prod[MAX_RHS + 5];
        scanf("%s", prod);
        if (!is_nonterminal(prod[0]) || prod[1] != '=') {
            printf("Invalid production: %s\n", prod);
            return 1;
        }
        lhs[i] = prod[0];
        strcpy(rhs[i], prod + 2);
        register_nt(lhs[i]);
        for (int j = 0; rhs[i][j]; ++j)
            if (is_nonterminal(rhs[i][j])) register_nt(rhs[i][j]);
    }

    recompute_first_fixed_point();
    compute_follow_fixed_point(lhs[0]);

    printf("\nFIRST sets\n");
    for (int i = 0; i < nt_count; ++i)
        printf("FIRST(%c) = { %s }\n", nonterminals[i], firstSet[nt_index(nonterminals[i])]);

    printf("\nFOLLOW sets\n");
    for (int i = 0; i < nt_count; ++i)
        printf("FOLLOW(%c) = { %s }\n", nonterminals[i], followSet[nt_index(nonterminals[i])]);

    return 0;
}
