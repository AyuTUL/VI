# Lab 17 — Front-End of a Mini Compiler

## Aim
To integrate lexical analysis, syntax analysis, a symbol table, elementary semantic checking, and intermediate-code generation into one mini compiler front-end.

## Language supported
The mini language supports:
- `int` and `float` declarations,
- comma-separated identifier declarations,
- arithmetic assignments,
- parentheses and `+ - * /`,
- `print(expression);`,
- `//` comments.

## Front-end flow
```text
Source Program
     |
     v
Flex Lexer  ---> tokens
     |
     v
Bison Parser ---> syntax structure
     |
     +------> Symbol Table / declaration checks
     |
     v
Three-Address Code
```

## Build and run
```bash
make
./minic < sample.mc
```

Or manually:
```bash
bison -d mini.y
flex mini.l
gcc mini.tab.c lex.yy.c symbol_table.c -o minic -lfl
./minic < sample.mc
```

## What the implementation demonstrates
1. Lexical recognition of keywords, identifiers, numbers, and punctuation.
2. Grammar-based syntax recognition.
3. Insertion and lookup of declared identifiers.
4. Detection of undeclared identifiers and redeclarations.
5. Generation of simple TAC with temporary variables.

## Limitations
This is an educational compiler front-end. It does not implement machine-code generation, nested scopes, function calls, arrays, full static type conversion, optimization, or error recovery beyond basic diagnostics.

## Viva questions
1. What is the role of the front-end of a compiler?
2. Why are Flex and Bison used together?
3. What information is stored in the symbol table?
4. What is the difference between syntax and semantic errors?
5. Why is intermediate code useful before target-code generation?
6. How would you extend this compiler to support `if` and `while`?
