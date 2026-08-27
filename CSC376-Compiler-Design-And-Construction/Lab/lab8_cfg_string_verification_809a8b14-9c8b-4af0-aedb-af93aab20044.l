%option noyywrap nodefault

%{
#include <bits/stdc++.h>
using namespace std;

enum TokenType {
    TOK_IDENTIFIER = 256,
    TOK_NUMBER,
    TOK_INVALID
};

static int lookahead;
static string currentLexeme;

static void advanceToken();
static void matchToken(int expected);
static void expression();
static void expressionPrime();
static void term();
static void termPrime();
static void factor();
%}

DIGIT       [0-9]
LETTER      [a-zA-Z_]
ID          {LETTER}({LETTER}|{DIGIT})*
NUMBER      {DIGIT}+(\.{DIGIT}+)?

%%

[ \t\r]+     { /* Ignore spaces. */ }
{NUMBER}     { return TOK_NUMBER; }
{ID}         { return TOK_IDENTIFIER; }
"+"          { return '+'; }
"-"          { return '-'; }
"*"          { return '*'; }
"/"          { return '/'; }
"("          { return '('; }
")"          { return ')'; }
\n           { return '\n'; }
.            { return TOK_INVALID; }

%%

static string tokenName(int token) {
    if (token == TOK_IDENTIFIER) return "identifier";
    if (token == TOK_NUMBER) return "number";
    if (token == TOK_INVALID) return "invalid token";
    if (token == 0) return "end of input";
    if (token == '\n') return "end of line";
    return string(1, static_cast<char>(token));
}

static void advanceToken() {
    lookahead = yylex();
    if (lookahead == 0) currentLexeme = "$";
    else currentLexeme.assign(yytext, yyleng);

    if (lookahead == TOK_INVALID)
        throw runtime_error("Invalid character '" + currentLexeme + "'");
}

static void matchToken(int expected) {
    if (lookahead != expected) {
        throw runtime_error(
            "Expected " + tokenName(expected) +
            " but found '" + currentLexeme + "'");
    }
    advanceToken();
}

/* E -> T E' */
static void expression() {
    term();
    expressionPrime();
}

/* E' -> + T E' | - T E' | epsilon */
static void expressionPrime() {
    while (lookahead == '+' || lookahead == '-') {
        advanceToken();
        term();
    }
}

/* T -> F T' */
static void term() {
    factor();
    termPrime();
}

/* T' -> * F T' | / F T' | epsilon */
static void termPrime() {
    while (lookahead == '*' || lookahead == '/') {
        advanceToken();
        factor();
    }
}

/* F -> ( E ) | identifier | number */
static void factor() {
    if (lookahead == TOK_IDENTIFIER) {
        matchToken(TOK_IDENTIFIER);
    } else if (lookahead == TOK_NUMBER) {
        matchToken(TOK_NUMBER);
    } else if (lookahead == '(') {
        matchToken('(');
        expression();
        matchToken(')');
    } else {
        throw runtime_error(
            "Expected identifier, number, or '(' but found '" +
            currentLexeme + "'");
    }
}

int main() {
    cout << "Grammar:\n"
         << "E  -> T E'\n"
         << "E' -> + T E' | - T E' | epsilon\n"
         << "T  -> F T'\n"
         << "T' -> * F T' | / F T' | epsilon\n"
         << "F  -> ( E ) | identifier | number\n\n";

    cout << "Enter expression: ";

    try {
        advanceToken();
        expression();

        if (lookahead == '\n') advanceToken();
        if (lookahead != 0) {
            throw runtime_error(
                "Extra token '" + currentLexeme + "' after expression");
        }

        cout << "String is VALID according to the CFG.\n";
    } catch (const exception &error) {
        cout << "String is INVALID: " << error.what() << '\n';
        return 1;
    }

    return 0;
}
