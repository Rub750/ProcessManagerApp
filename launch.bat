@echo off
setlocal

REM Launch script for Process Manager Pro
REM This script builds and runs the application

cd /d %~dp0

echo Building Process Manager Pro...
call build.ps1 -Configuration Release -SelfContained true

if %errorlevel% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo Launching application...
start .\bin\Release\net8.0-windows\win-x64\publish\ProcessManagerApp.exe

echo Application launched successfully!
pause
