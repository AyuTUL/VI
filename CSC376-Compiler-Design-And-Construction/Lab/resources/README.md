# Compiler Design and Construction — Laboratory Package (Labs 09–17)

This package contains complete laboratory material for **BSc.CSIT VI Semester — Compiler Design and Construction**, covering Lab 09 through Lab 17.

## Contents

| Lab | Topic | Main implementation |
|---|---|---|
| 09 | FIRST and FOLLOW sets | C |
| 10 | LL(1) checking and predictive parsing table | C |
| 11 | Recursive Descent Parser | C |
| 12 | Shift-Reduce Parser | C + Flex/Bison |
| 13 | Syntax Tree / DAG | C + Flex/Bison |
| 14 | Three-Address Code / Quadruples | Flex/Bison |
| 15 | Symbol Table | C + optional Flex |
| 16 | TAC for if-else / while using backpatching | Flex/Bison |
| 17 | Mini Compiler Front-End | Flex/Bison + Symbol Table + TAC |

## Recommended tools

### Ubuntu / Debian / WSL
```bash
sudo apt update
sudo apt install build-essential flex bison
```

### Windows
Use one of the following:
- WSL2 + Ubuntu (recommended), or
- MSYS2/MinGW with Flex and Bison installed.

## General Flex/Bison build pattern
```bash
bison -d file.y
flex file.l
gcc file.tab.c lex.yy.c -o program -lfl
./program
```

Each lab folder contains a dedicated `README.md` with theory, algorithm, build commands, sample input/output, expected learning outcome, and viva questions.

> Note: Source files are intentionally written in a classroom-friendly style. They favor readability and traceability over production-compiler complexity.
