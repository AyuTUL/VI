#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <stdlib.h>

/* Grammar:
   E  -> T E'
   E' -> + T E' | - T E' | epsilon
   T  -> F T'
   T' -> * F T' | / F T' | epsilon
   F  -> ( E ) | id | number
*/

char input[512];
int pos = 0;

void skip_ws(void){ while(isspace((unsigned char)input[pos])) pos++; }
char look(void){ skip_ws(); return input[pos]; }
void error(const char *msg){ printf("Rejected near position %d: %s\n",pos,msg); exit(1); }

void E(void); void Ep(void); void T(void); void Tp(void); void F(void);

void identifier_or_number(void){
    skip_ws();
    if(isalpha((unsigned char)input[pos]) || input[pos]=='_'){
        pos++;
        while(isalnum((unsigned char)input[pos]) || input[pos]=='_') pos++;
        return;
    }
    if(isdigit((unsigned char)input[pos])){
        pos++;
        while(isdigit((unsigned char)input[pos]) || input[pos]=='.') pos++;
        return;
    }
    error("expected identifier, number, or '('");
}

void F(void){
    skip_ws();
    if(input[pos]=='('){ pos++; E(); skip_ws(); if(input[pos]!=')') error("missing ')'"); pos++; }
    else identifier_or_number();
}

void Tp(void){
    skip_ws();
    while(input[pos]=='*' || input[pos]=='/'){
        pos++; F(); skip_ws();
    }
}

void T(void){ F(); Tp(); }

void Ep(void){
    skip_ws();
    while(input[pos]=='+' || input[pos]=='-'){
        pos++; T(); skip_ws();
    }
}

void E(void){ T(); Ep(); }

int main(void){
    printf("Enter arithmetic expression: ");
    if(!fgets(input,sizeof(input),stdin)) return 1;
    input[strcspn(input,"\n")]='\0';
    E(); skip_ws();
    if(input[pos]=='\0') printf("Accepted: the expression follows the grammar.\n");
    else printf("Rejected: unexpected token '%c' at position %d.\n",input[pos],pos);
    return 0;
}
