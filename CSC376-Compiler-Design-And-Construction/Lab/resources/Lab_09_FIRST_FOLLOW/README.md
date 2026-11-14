# Lab 09 — Computation of FIRST and FOLLOW Sets

## Aim
To write a C program that computes the **FIRST** and **FOLLOW** sets of all non-terminals in a context-free grammar.

## Theory
`FIRST(X)` is the set of terminals that can appear as the first symbol of a string derived from `X`. If `X` can derive the empty string, epsilon is also included. This program represents epsilon by `#`.

`FOLLOW(A)` is the set of terminals that can occur immediately to the right of non-terminal `A` in some sentential form. The end marker `$` is placed in `FOLLOW(S)` for the start symbol `S`.

The implementation repeatedly applies the standard FIRST and FOLLOW rules until no set changes. This fixed-point approach is suitable for a classroom implementation because it directly mirrors the mathematical definition.

## Input convention
Enter one alternative per line. For example, write `E=TX`, `X=+TX`, `X=#` instead of `E=TX` and `X=+TX|#`.

## Algorithm
1. Read all grammar productions.
2. Collect non-terminals.
3. Initialize all FIRST and FOLLOW sets as empty.
4. Repeatedly compute FIRST sets until no set changes.
5. Insert `$` into FOLLOW of the start symbol.
6. For each occurrence `A -> αBβ`, add `FIRST(β)-{ε}` to `FOLLOW(B)`.
7. If `β` is nullable or absent, add `FOLLOW(A)` to `FOLLOW(B)`.
8. Repeat FOLLOW propagation until stable.
9. Display the sets.

## Build and run
```bash
gcc first_follow.c -o first_follow
./first_follow < sample_input.txt
```

## Expected learning outcome
The student should be able to explain nullable productions, distinguish FIRST from FOLLOW, and apply both sets in predictive parsing.

## Viva questions
1. Why is `$` included only in the FOLLOW set of the start symbol initially?
2. Can epsilon appear in a FOLLOW set? Why or why not?
3. What happens when every symbol in `β` can derive epsilon?
4. Why are FIRST and FOLLOW computations often implemented iteratively?
5. How are FIRST and FOLLOW used in construction of an LL(1) table?
