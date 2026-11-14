# Lab 13 — Syntax Tree / DAG for an Arithmetic Expression

## Aim
To construct an abstract syntax tree (AST) for an arithmetic expression and understand how a DAG can share repeated subexpressions.

## Theory
A syntax tree removes unnecessary grammar symbols and preserves the essential hierarchical structure of operators and operands. Internal nodes normally represent operators while leaves represent identifiers or constants.

A **Directed Acyclic Graph (DAG)** extends this idea by allowing structurally identical subexpressions to share one node. For example, in `a*b + a*b`, an optimizer can represent both occurrences of `a*b` by a single DAG node.

## Files
- `ast_standalone.c` — direct recursive parser and tree printer.
- `ast.l`, `ast.y` — Flex/Bison AST construction.

## Build standalone
```bash
gcc ast_standalone.c -o ast
./ast
```

## Build Flex/Bison
```bash
bison -d ast.y
flex ast.l
gcc ast.tab.c lex.yy.c -o ast_bison -lfl
./ast_bison
```

## Viva questions
1. What is the difference between a parse tree and an abstract syntax tree?
2. Why can a DAG be more compact than a syntax tree?
3. How can a DAG support common-subexpression elimination?
4. Which nodes represent operands in an AST?
5. Why is precedence naturally visible in the tree structure?
