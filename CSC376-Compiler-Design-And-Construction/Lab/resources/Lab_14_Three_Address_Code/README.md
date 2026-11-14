# Lab 14 — Three-Address Code and Quadruples

## Aim
To generate three-address code (TAC) in quadruple form for assignment statements.

## Theory
Three-address code limits each instruction to at most one operator on the right-hand side. Complex expressions are decomposed into temporary values, such as:
```text
x = a + b * c
```
becoming conceptually:
```text
t1 = b * c
t2 = a + t1
x  = t2
```

A quadruple stores each instruction as `(operator, arg1, arg2, result)`.

## Build
```bash
bison -d tac.y
flex tac.l
gcc tac.tab.c lex.yy.c -o tac -lfl
./tac < sample_input.txt
```

## Viva questions
1. Why is TAC useful as an intermediate representation?
2. What are quadruples, triples, and indirect triples?
3. Why are temporary variables introduced?
4. How does expression-tree structure influence TAC order?
5. Where does TAC fit in the compiler pipeline?
