@echo off
setlocal

set "BISON_DIR=D:\Ayu\CSIT\Flex Windows\Bison"
set "LEX_DIR=D:\Ayu\CSIT\Flex Windows\Lex"
set "GCC=D:\Ayu\CSIT\MSYS2\mingw64\bin\gcc.exe"

set "NAME=context-free-grammar"

cd /d "%~dp0"
subst X: "%BISON_DIR%\.." >nul

X:\Bison\bin\bison.exe -d -o "%NAME%.tab.c" "%NAME%.y"
if errorlevel 1 goto error

X:\Lex\bin\flex.exe "%NAME%.l"
if errorlevel 1 goto error

"%GCC%" -std=c99 "%NAME%.tab.c" lex.yy.c -o "%NAME%.exe"
if errorlevel 1 goto error

echo Compilation successful: %NAME%.exe
goto end

:error
echo Compilation failed.
exit /b 1

:end
subst X: /d >nul
pause