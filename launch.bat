@echo off
chcp 65001 >nul 2>&1

echo ========================================
echo   ProcessManagerApp - Launch
echo ========================================
echo.

REM Try to find dotnet.exe
set DOTNET_CMD=dotnet
where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    REM Try common installation paths
    if exist "C:\Program Files\dotnet\dotnet.exe" (
        set DOTNET_CMD="C:\Program Files\dotnet\dotnet.exe"
    ) else if exist "C:\Program Files (x86)\dotnet\dotnet.exe" (
        set DOTNET_CMD="C:\Program Files (x86)\dotnet\dotnet.exe"
    ) else (
        echo ERROR: .NET SDK is not installed or not in PATH
        echo.
        echo Please install .NET 8.0+ SDK from: https://dotnet.microsoft.com/download
        echo.
        echo After installation, try again or restart your computer.
        echo.
        echo Common installation paths:
        echo   - C:\Program Files\dotnet\dotnet.exe
        echo   - C:\Program Files (x86)\dotnet\dotnet.exe
        pause
        exit /b 1
    )
)

REM Get dotnet version
for /f "tokens=*" %%i in ('%DOTNET_CMD% --version') do set DOTNET_VERSION=%%i

REM Extract major version number (ex: 10 from 10.0.401)
for /f "tokens=1 delims=. " %%a in ("%DOTNET_VERSION%") do set MAJOR_VERSION=%%a

REM Check if version is >= 8
if %MAJOR_VERSION% LSS 8 (
    echo ERROR: .NET 8.0 or higher is required (detected version: %DOTNET_VERSION%)
    pause
    exit /b 1
)

echo Detected .NET version: %DOTNET_VERSION%
echo.

REM Restore NuGet packages
echo Restoring NuGet packages...
%DOTNET_CMD% restore ProcessManagerApp.sln
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

REM Build the project in Release mode
echo Building the project...
%DOTNET_CMD% build ProcessManagerApp.sln --configuration Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

REM Find the executable
set EXE_PATH=bin\Release\net8.0-windows\ProcessManagerApp.exe
if not exist "%EXE_PATH%" (
    echo ERROR: Executable not found: %EXE_PATH%
    pause
    exit /b 1
)

echo.
echo Launching ProcessManagerApp...
echo.

REM Launch the application
start "" "%EXE_PATH%"

echo.
echo Application launched successfully!
echo You can close this window.
pause
