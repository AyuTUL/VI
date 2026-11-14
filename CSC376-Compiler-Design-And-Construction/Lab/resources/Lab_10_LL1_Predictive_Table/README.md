# Lab 10 — LL(1) Checking and Predictive Parsing Table

## Aim
To determine whether a grammar is LL(1) and to construct its predictive parsing table.

## Core idea
For every production `A -> α`:
- Put `A -> α` in `M[A,a]` for each `a` in `FIRST(α)-{ε}`.
- If `ε` belongs to `FIRST(α)`, also put the production in `M[A,b]` for every `b` in `FOLLOW(A)`.
- If one table cell requires more than one distinct production, the grammar has an LL(1) conflict.

## Algorithm
1. Read the grammar.
2. Compute FIRST and FOLLOW sets.
3. Create an initially empty parsing table.
4. Insert productions using FIRST and FOLLOW rules.
5. Detect duplicate/conflicting entries.
6. Print the parsing table and LL(1) result.

## Build
```bash
gcc ll1_table.c -o ll1
./ll1 < sample_input.txt
```

## Important academic note
An LL(1) grammar allows a predictive parser to select one production using one look-ahead token. Left recursion and common prefixes normally prevent direct LL(1) parsing until the grammar is transformed.

## Viva questions
1. What does each `L` in LL(1) represent?
2. Why does left recursion cause a problem for predictive parsing?
3. What is a FIRST/FIRST conflict?
4. What is a FIRST/FOLLOW conflict?
5. Why is left factoring useful before constructing an LL(1) table?
