%code requires {
struct Node;
}
%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
typedef struct Node Node;
struct Node { char label[32]; struct Node *left,*right; };
Node *root;
int yylex(void); void yyerror(const char *s);
Node *mk(const char *lab, Node *l, Node *r){ Node*n=malloc(sizeof(*n)); snprintf(n->label,sizeof(n->label),"%s",lab); n->left=l;n->right=r;return n; }

#define MAX_NODES 128
#define MAX_ROWS  64
#define MAX_WIDTH 2048
typedef struct { Node *node; int x, depth; } Placed;
static Placed placed[MAX_NODES];
static int node_count, next_x, max_depth;

static int place(Node *n, int depth) {
    int left, right, x;
    if (!n) return -1;
    if (depth > max_depth) max_depth = depth;
    left = place(n->left, depth + 1);
    right = place(n->right, depth + 1);
    if (left >= 0 && right >= 0) x = (left + right) / 2;
    else if (left >= 0) x = left;
    else if (right >= 0) x = right;
    else {
        x = next_x;
        next_x += (int)strlen(n->label) + 4;
    }
    placed[node_count++] = (Placed){ n, x, depth };
    return x;
}

static int position(Node *n) {
    for (int i = 0; i < node_count; i++)
        if (placed[i].node == n) return placed[i].x;
    return -1;
}

static void print_tree(Node *root) {
    char canvas[MAX_ROWS][MAX_WIDTH];
    memset(canvas, ' ', sizeof(canvas));
    node_count = next_x = max_depth = 0;
    place(root, 0);

    for (int i = 0; i < node_count; i++) {
        Node *n = placed[i].node;
        int row = placed[i].depth * 2;
        int start = placed[i].x - (int)strlen(n->label) / 2;
        for (int j = 0; n->label[j] && start + j < MAX_WIDTH; j++)
            if (start + j >= 0) canvas[row][start + j] = n->label[j];
        if (n->left) canvas[row + 1][(placed[i].x + position(n->left)) / 2] = '/';
        if (n->right) canvas[row + 1][(placed[i].x + position(n->right)) / 2] = '\\';
    }
    for (int row = 0; row <= max_depth * 2; row++) {
        int end = MAX_WIDTH - 1;
        while (end >= 0 && canvas[row][end] == ' ') end--;
        if (end >= 0) printf("%.*s\n", end + 1, canvas[row]);
    }
}
%}
%union { char *str; struct Node *node; }
%token <str> VALUE
%token END_LINE
%type <node> expr term factor
%left '+' '-'
%left '*' '/'
%%
input : expr END_LINE { root=$1; }
      ;
expr  : expr '+' term { $$=mk("+",$1,$3); }
      | expr '-' term { $$=mk("-",$1,$3); }
      | term          { $$=$1; }
      ;
term  : term '*' factor { $$=mk("*",$1,$3); }
      | term '/' factor { $$=mk("/",$1,$3); }
      | factor          { $$=$1; }
      ;
factor: '(' expr ')' { $$=$2; }
      | VALUE        { $$=mk($1,NULL,NULL); free($1); }
      ;
%%
void yyerror(const char*s){ fprintf(stderr,"Parse error: %s\n",s); }
int main(void) {
    printf("Enter expression: ");
    if (yyparse() == 0) {
        puts("\nSyntax Tree:");
        print_tree(root);
    }
    return 0;
}
