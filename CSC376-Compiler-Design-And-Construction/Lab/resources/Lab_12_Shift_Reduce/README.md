# Lab 12 — Shift-Reduce (Bottom-Up) Parsing

## Aim
To implement and observe bottom-up shift-reduce parsing for arithmetic expressions.

## Theory
A shift-reduce parser maintains a stack and an unread input buffer. It repeatedly performs one of four conceptual actions: **shift**, **reduce**, **accept**, or **error**. Bison/YACC generates an LALR(1) bottom-up parser and is therefore a practical way to study shift-reduce behavior.

This lab contains two implementations:
- `shift_reduce_demo.c` — classroom demonstration of stack actions using the simplified symbol `i` for identifiers.
- `calc.l` + `calc.y` — recommended Flex/Bison implementation with precedence and associativity declarations.

## Build C demonstration
```bash
gcc shift_reduce_demo.c -o shift_demo
./shift_demo
```

## Build Flex/Bison version
```bash
bison -d calc.y
flex calc.l
gcc calc.tab.c lex.yy.c -o shift_reduce -lfl
./shift_reduce
```

## Viva questions
1. What is a handle in bottom-up parsing?
2. What is a shift-reduce conflict?
3. What is a reduce-reduce conflict?
4. Why are precedence declarations useful in YACC/Bison?
5. How does bottom-up parsing differ from recursive descent?
