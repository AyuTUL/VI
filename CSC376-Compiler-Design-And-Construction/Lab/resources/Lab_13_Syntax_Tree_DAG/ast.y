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
void print_tree(Node*n,int d){ if(!n)return; print_tree(n->right,d+1); for(int i=0;i<d;i++)printf("    "); printf("%s\n",n->label); print_tree(n->left,d+1); }
%}
%union { char *str; struct Node *node; }
%token <str> VALUE
%type <node> expr term factor
%left '+' '-'
%left '*' '/'
%%
input : expr { root=$1; }
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
int main(void){ printf("Enter expression: "); if(yyparse()==0){ puts("\nSyntax Tree:"); print_tree(root,0);} return 0; }
