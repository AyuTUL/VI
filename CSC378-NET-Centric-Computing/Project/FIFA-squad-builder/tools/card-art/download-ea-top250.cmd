@echo off
setlocal

cd /d "%~dp0"

set "NODE_EXE=node"
where node >nul 2>nul
if errorlevel 1 (
  if exist "D:\Ayu\CSIT\NodeJS\node.exe" (
    set "NODE_EXE=D:\Ayu\CSIT\NodeJS\node.exe"
  )
)

"%NODE_EXE%" download-ea-cards.mjs --top 250
pause
