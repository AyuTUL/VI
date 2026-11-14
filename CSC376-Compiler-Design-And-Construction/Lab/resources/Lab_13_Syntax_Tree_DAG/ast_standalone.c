#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>

typedef struct Node { char op; char text[32]; struct Node *l,*r; } Node;
char s[512]; int p=0;

void ws(){ while(isspace((unsigned char)s[p])) p++; }
Node *node(char op,const char *text,Node*l,Node*r){ Node*n=malloc(sizeof(*n)); n->op=op; snprintf(n->text,sizeof(n->text),"%s",text?text:""); n->l=l;n->r=r;return n; }
Node *E(void); Node *T(void); Node *F(void);
Node *F(void){ ws(); if(s[p]=='('){p++;Node*n=E();ws();if(s[p]==')')p++;return n;} char buf[32]="";int k=0;while(isalnum((unsigned char)s[p])||s[p]=='_') if(k<31)buf[k++]=s[p++]; else p++; return node('v',buf,NULL,NULL); }
Node *T(void){ Node*n=F();ws();while(s[p]=='*'||s[p]=='/'){char o=s[p++];Node*r=F();n=node(o,"",n,r);ws();}return n; }
Node *E(void){ Node*n=T();ws();while(s[p]=='+'||s[p]=='-'){char o=s[p++];Node*r=T();n=node(o,"",n,r);ws();}return n; }
void print(Node*n,int d){ if(!n)return; print(n->r,d+1); for(int i=0;i<d;i++)printf("    "); if(n->op=='v')printf("%s\n",n->text); else printf("%c\n",n->op); print(n->l,d+1); }
void post(Node*n){ if(!n)return;post(n->l);post(n->r);if(n->op=='v')printf("%s ",n->text);else printf("%c ",n->op); }
int main(){printf("Expression: ");fgets(s,sizeof(s),stdin);Node*root=E();printf("\nSyntax tree (sideways):\n");print(root,0);printf("\nPostorder: ");post(root);printf("\n");return 0;}
