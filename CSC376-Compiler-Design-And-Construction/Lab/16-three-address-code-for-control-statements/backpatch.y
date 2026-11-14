%code requires {
struct List;
struct BoolAttr;
struct StmtAttr;
}
%{
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdarg.h>

typedef struct List List;
typedef struct BoolAttr BoolAttr;
typedef struct StmtAttr StmtAttr;
struct List { int index; struct List *next; };
struct BoolAttr { List *truelist, *falselist; };
struct StmtAttr { List *nextlist; };

typedef struct { char text[128]; } Instr;
Instr code[256];
int nextquad = 0, tempno = 0;

int yylex(void);
void yyerror(const char *s);

List *makelist(int i){
    List *n = malloc(sizeof(*n));
    n->index = i; n->next = NULL;
    return n;
}

List *merge(List *a, List *b){
    if(!a) return b;
    List *p = a;
    while(p->next) p = p->next;
    p->next = b;
    return a;
}

void backpatch(List *l, int target){
    for(; l; l = l->next){
        char *blank = strchr(code[l->index].text, '_');
        if(blank){
            char target_text[24];
            sprintf(target_text, "%d", target);
            *blank = '\0';
            strncat(code[l->index].text, target_text,
                    sizeof(code[l->index].text) - strlen(code[l->index].text) - 1);
        }
    }
}

int emit(const char *fmt, ...){
    va_list ap;
    va_start(ap, fmt);
    vsnprintf(code[nextquad].text, sizeof(code[nextquad].text), fmt, ap);
    va_end(ap);
    return nextquad++;
}

char *newtemp(void){
    char b[20]; sprintf(b, "t%d", ++tempno);
    return strdup(b);
}

char *gen(char *a, const char *op, char *b){
    char *t = newtemp();
    emit("%s = %s %s %s", t, a, op, b);
    free(a); free(b);
    return t;
}
%}

%define parse.error verbose
%union { char *str; int quad; struct BoolAttr *b; struct StmtAttr *s; }
%token IF ELSE WHILE
%token <str> ID ATOM RELOP
%type <str> expr term factor
%type <b> bool
%type <s> stmt stmts N
%type <quad> M

%%
program : stmts {
            backpatch($1->nextlist, nextquad);
          }
        ;

stmts : stmts M stmt {
          backpatch($1->nextlist, $2);
          $$ = $3;
        }
      | stmt { $$ = $1; }
      ;

M : /* empty */ { $$ = nextquad; }
  ;

/* N emits the jump that skips the else-part after the then-part. */
N : /* empty */ {
      $$ = malloc(sizeof(StmtAttr));
      $$->nextlist = makelist(emit("goto _"));
    }
  ;

stmt : ID '=' expr ';' {
         emit("%s = %s", $1, $3);
         free($1); free($3);
         $$ = malloc(sizeof(StmtAttr));
         $$->nextlist = NULL;
       }
     | IF '(' bool ')' M stmt N ELSE M stmt {
         backpatch($3->truelist, $5);
         backpatch($3->falselist, $9);
         $$ = malloc(sizeof(StmtAttr));
         $$->nextlist = merge($6->nextlist,
                              merge($7->nextlist, $10->nextlist));
       }
     | WHILE M '(' bool ')' M stmt {
         backpatch($4->truelist, $6);
         backpatch($7->nextlist, $2);
         emit("goto %d", $2);
         $$ = malloc(sizeof(StmtAttr));
         $$->nextlist = $4->falselist;
       }
     | '{' stmts '}' { $$ = $2; }
     ;

bool : expr RELOP expr {
         $$ = malloc(sizeof(BoolAttr));
         int q1 = emit("if %s %s %s goto _", $1, $2, $3);
         int q2 = emit("goto _");
         $$->truelist = makelist(q1);
         $$->falselist = makelist(q2);
         free($1); free($2); free($3);
       }
     ;

expr : expr '+' term { $$ = gen($1, "+", $3); }
     | expr '-' term { $$ = gen($1, "-", $3); }
     | term          { $$ = $1; }
     ;

term : term '*' factor { $$ = gen($1, "*", $3); }
     | term '/' factor { $$ = gen($1, "/", $3); }
     | factor          { $$ = $1; }
     ;

factor : '(' expr ')' { $$ = $2; }
       | ID           { $$ = $1; }
       | ATOM         { $$ = $1; }
       ;
%%

void yyerror(const char *s){
    fprintf(stderr, "Parse error: %s\n", s);
}

int main(void){
    puts("Enter statements using if/else/while; braces are supported.");
    if(yyparse() == 0){
        puts("\nGenerated TAC after backpatching:");
        for(int i = 0; i < nextquad; ++i)
            printf("%3d: %s\n", i, code[i].text);
    }
    return 0;
}
