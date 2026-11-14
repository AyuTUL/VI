@echo off
setlocal
set "STATUS=0"
set "BISON_DIR=D:\Ayu\CSIT\Flex Windows\Bison"
set "GCC=D:\Ayu\CSIT\MSYS2\mingw64\bin\gcc.exe"
set "NAME=syntax-tree-for-arithmetric-expressions-old"

cd /d "%~dp0"
subst X: "%BISON_DIR%\.." >nul
X:\Bison\bin\bison.exe -d -o "%NAME%.tab.c" "%NAME%.y" || set "STATUS=1"
if "%STATUS%"=="0" X:\Lex\bin\flex.exe "%NAME%.l" || set "STATUS=1"
if "%STATUS%"=="0" "%GCC%" -std=c99 "%NAME%.tab.c" lex.yy.c -o "%NAME%.exe" || set "STATUS=1"
if "%STATUS%"=="0" echo Compilation successful: %NAME%.exe
if not "%STATUS%"=="0" echo Compilation failed.
subst X: /d >nul
pause
exit /b %STATUS%
