# Lab 11 — Recursive Descent Parser

## Aim
To implement a top-down recursive descent parser for arithmetic expressions.

## Grammar used
```text
E  -> T E'
E' -> + T E' | - T E' | ε
T  -> F T'
T' -> * F T' | / F T' | ε
F  -> ( E ) | id | number
```

The grammar is suitable for recursive descent because immediate left recursion has been removed. Each major non-terminal is represented by a C function.

## Working principle
The parser starts at `E()`. `E()` calls `T()`, which calls `F()`. The current input position acts as the look-ahead pointer. If a required symbol is absent, the parser reports a syntax error. Acceptance occurs only when the start non-terminal completes and the entire input has been consumed.

## Build and run
```bash
gcc recursive_descent.c -o rdp
./rdp
```

### Example
Input: `a + b * (c - 2)`

Expected result: accepted.

## Viva questions
1. Why must left recursion be removed for a recursive descent parser?
2. What is the role of look-ahead?
3. What is the difference between recursive descent and predictive parsing?
4. How is operator precedence represented in the grammar?
5. What causes backtracking in a general top-down parser?
