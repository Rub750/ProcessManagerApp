@echo off
setlocal

REM Process Manager Pro - Launch Script
REM This script builds and runs the application

cd /d %~dp0

echo Process Manager Pro - Starting...

REM Check if publish directory exists and has the exe
if exist .\publish\ProcessManagerApp.exe (
    echo Launching existing build...
    start "" .\publish\ProcessManagerApp.exe
    echo Application launched!
    exit /b 0
)

echo Building application (first run - this may take a moment)...

REM Build with .NET CLI
dotnet restore ProcessManagerApp.csproj >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: Failed to restore packages
    echo Please ensure .NET 8.0 SDK is installed
    echo Download from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

dotnet build ProcessManagerApp.csproj -c Release --no-restore >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: Failed to build project
    pause
    exit /b 1
)

dotnet publish ProcessManagerApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false -o .\publish >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: Failed to publish application
    pause
    exit /b 1
)

echo Launching Process Manager Pro...
start "" .\publish\ProcessManagerApp.exe

echo Application launched successfully!
echo You can close this window.
