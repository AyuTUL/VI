#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "symbol_table.h"
#define H 37
typedef struct S { char name[64], type[16]; struct S*next; } S;
static S*tab[H];
static unsigned hs(const char*s){unsigned h=5381;while(*s)h=((h<<5)+h)+(unsigned char)*s++;return h%H;}
int st_insert(const char*n,const char*t){unsigned k=hs(n);for(S*p=tab[k];p;p=p->next)if(!strcmp(p->name,n))return 0;S*x=malloc(sizeof(*x));snprintf(x->name,sizeof(x->name),"%s",n);snprintf(x->type,sizeof(x->type),"%s",t);x->next=tab[k];tab[k]=x;return 1;}
const char *st_type(const char*n){for(S*p=tab[hs(n)];p;p=p->next)if(!strcmp(p->name,n))return p->type;return NULL;}
void st_print(void){puts("\nSYMBOL TABLE\nName                 Type\n---------------------------");for(int i=0;i<H;i++)for(S*p=tab[i];p;p=p->next)printf("%-20s %s\n",p->name,p->type);}
