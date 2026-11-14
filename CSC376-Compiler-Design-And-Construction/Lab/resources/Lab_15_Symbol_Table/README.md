# Lab 15 — Symbol Table

## Aim
To implement a symbol table using a hash table and linked-list collision handling.

## Theory
A symbol table stores attributes of program entities such as identifiers, functions, constants, and types. Typical attributes include name, type, scope, storage information, and source location. Compiler phases query and update this table during semantic analysis and code generation.

## Files
- `symbol_table.c` — menu-driven hash-table implementation.
- `symtab.l` — optional Flex program that automatically collects identifiers from C-like input.

## Build C version
```bash
gcc symbol_table.c -o symbol_table
./symbol_table
```

## Build Lex version
```bash
flex symtab.l
gcc lex.yy.c -o symlex -lfl
./symlex < sample_program.c
```

## Viva questions
1. Why is a hash table appropriate for a symbol table?
2. What is a hash collision?
3. What information can be stored for an identifier?
4. Why is scope important?
5. Which compiler phases use the symbol table?
