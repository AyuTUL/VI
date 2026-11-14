#include <stdio.h>
#include <string.h>
#include <ctype.h>

#define MAXP 60
#define MAXR 80
#define MAXSET 80
#define MAXT 80

char L[MAXP], R[MAXP][MAXR];
int n;
char nts[30] = "", terms[MAXT] = "";
char FIRST[26][MAXSET], FOLLOW[26][MAXSET];
char table[26][128][MAXR];

int isNT(char c){ return c >= 'A' && c <= 'Z'; }
int idx(char c){ return c - 'A'; }
void add(char *s, char c){ if(!strchr(s,c)){ size_t k=strlen(s); s[k]=c; s[k+1]='\0'; } }
int eps(const char *s){ return strchr(s,'#') != NULL; }

void collect(void){
    for(int p=0;p<n;p++){
        add(nts,L[p]);
        for(int j=0;R[p][j];j++){
            char c=R[p][j];
            if(isNT(c)) add(nts,c);
            else if(c!='#') add(terms,c);
        }
    }
    add(terms,'$');
}

void first_string(const char *s, char *out){
    out[0]='\0';
    if(!s[0] || (s[0]=='#' && !s[1])){ add(out,'#'); return; }
    int nullable=1;
    for(int i=0;s[i];i++){
        char x=s[i];
        if(!isNT(x)){ add(out,x); nullable=0; break; }
        for(int k=0;FIRST[idx(x)][k];k++) if(FIRST[idx(x)][k]!='#') add(out,FIRST[idx(x)][k]);
        if(!eps(FIRST[idx(x)])){ nullable=0; break; }
    }
    if(nullable) add(out,'#');
}

void compute_first(void){
    int changed;
    do{
        char old[26][MAXSET]; memcpy(old,FIRST,sizeof(old));
        for(int p=0;p<n;p++){
            char temp[MAXSET]=""; first_string(R[p],temp);
            for(int k=0;temp[k];k++) add(FIRST[idx(L[p])],temp[k]);
        }
        changed=memcmp(old,FIRST,sizeof(old))!=0;
    }while(changed);
}

void compute_follow(void){
    add(FOLLOW[idx(L[0])],'$');
    int changed;
    do{
        char old[26][MAXSET]; memcpy(old,FOLLOW,sizeof(old));
        for(int p=0;p<n;p++){
            int len=(int)strlen(R[p]);
            for(int i=0;i<len;i++) if(isNT(R[p][i])){
                char B=R[p][i], beta[MAXR]; strcpy(beta,R[p]+i+1);
                char fb[MAXSET]=""; first_string(beta,fb);
                for(int k=0;fb[k];k++) if(fb[k]!='#') add(FOLLOW[idx(B)],fb[k]);
                if(!beta[0] || eps(fb))
                    for(int k=0;FOLLOW[idx(L[p])][k];k++) add(FOLLOW[idx(B)],FOLLOW[idx(L[p])][k]);
            }
        }
        changed=memcmp(old,FOLLOW,sizeof(old))!=0;
    }while(changed);
}

int put_cell(char A, char a, const char *prod){
    char *cell=table[idx(A)][(unsigned char)a];
    if(cell[0] && strcmp(cell,prod)!=0) return 0;
    strcpy(cell,prod); return 1;
}

int construct_table(void){
    int ll1=1;
    for(int p=0;p<n;p++){
        char f[MAXSET]=""; first_string(R[p],f);
        char prod[MAXR+4]; snprintf(prod,sizeof(prod),"%c=%s",L[p],R[p]);
        for(int k=0;f[k];k++) if(f[k]!='#')
            if(!put_cell(L[p],f[k],prod)) ll1=0;
        if(eps(f)){
            for(int k=0;FOLLOW[idx(L[p])][k];k++)
                if(!put_cell(L[p],FOLLOW[idx(L[p])][k],prod)) ll1=0;
        }
    }
    return ll1;
}

void print_sets(void){
    for(int i=0;nts[i];i++) printf("FIRST(%c)={%s}  FOLLOW(%c)={%s}\n",nts[i],FIRST[idx(nts[i])],nts[i],FOLLOW[idx(nts[i])]);
}

void print_table(void){
    printf("\nPredictive Parsing Table\nNT\\T\t");
    for(int j=0;terms[j];j++) printf("%c\t",terms[j]);
    printf("\n");
    for(int i=0;nts[i];i++){
        char A=nts[i]; printf("%c\t",A);
        for(int j=0;terms[j];j++){
            char *c=table[idx(A)][(unsigned char)terms[j]];
            printf("%s\t", c[0]?c:"-");
        }
        printf("\n");
    }
}

int main(void){
    printf("Number of productions: ");
    if(scanf("%d",&n)!=1 || n<=0 || n>MAXP) return 1;
    printf("Enter one alternative per line, e.g. E=TX and X=#\n");
    for(int i=0;i<n;i++){
        char p[MAXR+4]; scanf("%s",p);
        if(!isNT(p[0]) || p[1]!='='){ printf("Invalid production.\n"); return 1; }
        L[i]=p[0]; strcpy(R[i],p+2);
    }
    collect(); compute_first(); compute_follow();
    print_sets();
    int ok=construct_table(); print_table();
    printf("\nResult: Grammar is %sLL(1).\n", ok?"":"NOT ");
    if(!ok) printf("Reason: at least one table cell has multiple possible productions.\n");
    return 0;
}
