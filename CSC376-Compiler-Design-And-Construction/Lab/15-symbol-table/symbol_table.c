#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define SIZE 31

typedef struct Symbol {
    char name[64], type[32], scope[32];
    int line;
    struct Symbol *next;
} Symbol;

Symbol *table[SIZE];

unsigned hash(const char *s){ unsigned h=5381; while(*s) h=((h<<5)+h)+(unsigned char)*s++; return h%SIZE; }
Symbol *lookup(const char *name){
    for(Symbol *p=table[hash(name)];p;p=p->next) if(strcmp(p->name,name)==0) return p;
    return NULL;
}
int insert(const char *name,const char *type,const char *scope,int line){
    if(lookup(name)) return 0;
    unsigned k=hash(name); Symbol*n=malloc(sizeof(*n));
    snprintf(n->name,sizeof(n->name),"%s",name); snprintf(n->type,sizeof(n->type),"%s",type);
    snprintf(n->scope,sizeof(n->scope),"%s",scope); n->line=line; n->next=table[k]; table[k]=n; return 1;
}
void display(void){
    puts("\nNAME                 TYPE           SCOPE          LINE");
    puts("----------------------------------------------------------");
    for(int i=0;i<SIZE;i++) for(Symbol*p=table[i];p;p=p->next)
        printf("%-20s %-14s %-14s %d\n",p->name,p->type,p->scope,p->line);
}
int main(void){
    int choice,line; char name[64],type[32],scope[32];
    while(1){
        printf("\n1.Insert  2.Search  3.Display  4.Exit\nChoice: ");
        if(scanf("%d",&choice)!=1) break;
        if(choice==1){
            printf("Name Type Scope Line: "); scanf("%63s%31s%31s%d",name,type,scope,&line);
            printf(insert(name,type,scope,line)?"Inserted.\n":"Duplicate symbol.\n");
        }else if(choice==2){
            printf("Name: "); scanf("%63s",name); Symbol*p=lookup(name);
            if(p) printf("Found: %s, type=%s, scope=%s, line=%d\n",p->name,p->type,p->scope,p->line);
            else puts("Not found.");
        }else if(choice==3) display();
        else if(choice==4) break;
        else puts("Invalid choice.");
    }
    return 0;
}
