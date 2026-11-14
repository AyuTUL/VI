#include <stdio.h>
#include <string.h>
#include <ctype.h>

/* Demonstration grammar: E -> E+E | E*E | (E) | i
   Input uses i for an identifier, e.g. i+i*i
   This program is for observing shift/reduce actions. The accompanying
   Bison version is the recommended conflict-resolved implementation.
*/

char stack[200] = "";
char input[200];
int ip = 0;

void show(const char *action){ printf("%-18s %-18s %s\n", stack, input+ip, action); }

int reduce_once(void){
    int n=(int)strlen(stack);
    if(n>=1 && stack[n-1]=='i') { stack[n-1]='E'; show("Reduce E->i"); return 1; }
    if(n>=3 && stack[n-3]=='(' && stack[n-2]=='E' && stack[n-1]==')') {
        stack[n-3]='E'; stack[n-2]='\0'; show("Reduce E->(E)"); return 1;
    }
    if(n>=3 && stack[n-3]=='E' && stack[n-2]=='*' && stack[n-1]=='E') {
        stack[n-3]='E'; stack[n-2]='\0'; show("Reduce E->E*E"); return 1;
    }
    /* Reduce + only when multiplication is not waiting in the unread input.
       This small condition demonstrates precedence in this educational parser. */
    if(n>=3 && stack[n-3]=='E' && stack[n-2]=='+' && stack[n-1]=='E' && input[ip] != '*') {
        stack[n-3]='E'; stack[n-2]='\0'; show("Reduce E->E+E"); return 1;
    }
    return 0;
}

int main(void){
    printf("Enter expression using i for identifiers (example i+i*i): ");
    scanf("%199s",input);
    printf("%-18s %-18s %s\n","STACK","INPUT","ACTION");
    show("Start");
    while(input[ip]){
        int n=(int)strlen(stack); stack[n]=input[ip++]; stack[n+1]='\0'; show("Shift");
        while(reduce_once());
    }
    while(reduce_once());
    if(strcmp(stack,"E")==0) show("ACCEPT"); else show("REJECT");
    return 0;
}
