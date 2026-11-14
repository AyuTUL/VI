# Lab 16 — TAC for Control Statements Using Backpatching

## Aim
To generate three-address code for `if-else` and `while` statements using backpatching. The supplied grammar intentionally uses the complete `if (...) statement else statement` form to keep the laboratory grammar deterministic and focused on backpatching.

## Theory
During one-pass intermediate-code generation, the destination of a forward jump may not yet be known. **Backpatching** solves this by emitting an incomplete jump and storing its instruction number in a list. When the target address becomes known, all instructions in that list are patched.

The standard attributes are:
- `truelist`: jumps taken when a Boolean expression is true.
- `falselist`: jumps taken when it is false.
- `nextlist`: jumps that must continue to the instruction after a statement.

## Build
```bash
bison -d backpatch.y
flex backpatch.l
gcc backpatch.tab.c lex.yy.c -o backpatch -lfl
./backpatch < sample_input.txt
```

## Viva questions
1. Why is backpatching needed?
2. What are `truelist`, `falselist`, and `nextlist`?
3. What does `makelist()` do?
4. What does `merge()` do?
5. How does backpatching differ for `if-else` and `while`?
