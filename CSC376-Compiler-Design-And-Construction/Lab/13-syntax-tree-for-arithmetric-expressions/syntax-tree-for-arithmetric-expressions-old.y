%code requires {
struct Node;
}
%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
typedef struct Node Node;
struct Node { char label[32]; struct Node *left, *right; };
Node *root;
int yylex(void);
void yyerror(const char *s);

Node *mk(const char *label, Node *left, Node *right) {
    Node *node = malloc(sizeof(*node));
    snprintf(node->label, sizeof(node->label), "%s", label);
    node->left = left;
    node->right = right;
    return node;
}

void print_tree(Node *node, int depth) {
    if (!node) return;
    print_tree(node->right, depth + 1);
    for (int i = 0; i < depth; i++) printf("    ");
    puts(node->label);
    print_tree(node->left, depth + 1);
}
%}
%union { char *str; struct Node *node; }
%token <str> VALUE
%token END_LINE
%type <node> expr term factor
%left '+' '-'
%left '*' '/'
%%
input : expr END_LINE { root = $1; };
expr : expr '+' term { $$ = mk("+", $1, $3); }
     | expr '-' term  { $$ = mk("-", $1, $3); }
     | term           { $$ = $1; };
term : term '*' factor { $$ = mk("*", $1, $3); }
     | term '/' factor { $$ = mk("/", $1, $3); }
     | factor          { $$ = $1; };
factor : '(' expr ')' { $$ = $2; }
       | VALUE         { $$ = mk($1, NULL, NULL); free($1); };
%%
void yyerror(const char *s) { fprintf(stderr, "Parse error: %s\n", s); }

int main(void) {
    printf("Enter expression: ");
    if (yyparse() == 0) {
        puts("\nSyntax Tree:");
        print_tree(root, 0);
    }
    return 0;
}
