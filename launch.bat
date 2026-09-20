@echo off
chcp 65001 >nul 2>&1

echo ========================================
echo   ProcessManagerApp - Launch
echo ========================================
echo.

REM ========================================
REM Find dotnet.exe
REM ========================================
echo Searching for dotnet...

set DOTNET_CMD=

REM Try PATH first
where dotnet >nul 2>&1
if %ERRORLEVEL% equ 0 (
    set DOTNET_CMD=dotnet
    echo Found dotnet in PATH
    goto CHECK_VERSION
)

REM Try common installation paths
if exist "C:\Program Files\dotnet\dotnet.exe" (
    set DOTNET_CMD="C:\Program Files\dotnet\dotnet.exe"
    echo Found dotnet at: C:\Program Files\dotnet\dotnet.exe
    goto CHECK_VERSION
)

if exist "C:\Program Files (x86)\dotnet\dotnet.exe" (
    set DOTNET_CMD="C:\Program Files (x86)\dotnet\dotnet.exe"
    echo Found dotnet at: C:\Program Files (x86)\dotnet\dotnet.exe
    goto CHECK_VERSION
)

REM Try user profile .dotnet
if exist "%USERPROFILE%\.dotnet\dotnet.exe" (
    set DOTNET_CMD="%USERPROFILE%\.dotnet\dotnet.exe"
    echo Found dotnet at: %USERPROFILE%\.dotnet\dotnet.exe
    goto CHECK_VERSION
)

echo.
echo ERROR: .NET SDK not found!
echo.
echo Searched in:
echo   - PATH environment variable
echo   - C:\Program Files\dotnet\dotnet.exe
echo   - C:\Program Files (x86)\dotnet\dotnet.exe
echo   - %USERPROFILE%\.dotnet\dotnet.exe
echo.
echo Please install .NET 8.0+ SDK from: https://dotnet.microsoft.com/download
echo.
echo After installation:
echo   1. Restart your computer
   2. Or manually add dotnet to PATH
   3. Or move this .bat file to the same folder as dotnet.exe
echo.
pause
exit /b 1

:CHECK_VERSION
echo.
echo Checking .NET version...
for /f "tokens=*" %%i in ('%DOTNET_CMD% --version 2^>nul') do set DOTNET_VERSION=%%i

if "%DOTNET_VERSION%"=="" (
    echo ERROR: Could not get .NET version
    echo Make sure %DOTNET_CMD% is the correct path to dotnet.exe
    pause
    exit /b 1
)

REM Extract major version number
for /f "tokens=1 delims=. " %%a in ("%DOTNET_VERSION%") do set MAJOR_VERSION=%%a

if %MAJOR_VERSION% LSS 8 (
    echo ERROR: .NET 8.0 or higher required (found: %DOTNET_VERSION%)
    pause
    exit /b 1
)

echo .NET version: %DOTNET_VERSION% (OK)
echo.

REM ========================================
REM Restore and Build
REM ========================================
echo Restoring NuGet packages...
%DOTNET_CMD% restore ProcessManagerApp.sln
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to restore packages
    echo Make sure you are in the project directory (where ProcessManagerApp.sln is)
    pause
    exit /b 1
)

echo Building project...
%DOTNET_CMD% build ProcessManagerApp.sln --configuration Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

REM ========================================
REM Launch
REM ========================================
echo.
echo Checking for executable...
set EXE_PATH=bin\Release\net8.0-windows\ProcessManagerApp.exe

if not exist "%EXE_PATH%" (
    echo ERROR: Executable not found at: %EXE_PATH%
    echo.
    echo Trying alternative paths...
    
    if exist "bin\Release\net8.0\ProcessManagerApp.exe" (
        set EXE_PATH=bin\Release\net8.0\ProcessManagerApp.exe
    ) else if exist "bin\Debug\net8.0-windows\ProcessManagerApp.exe" (
        set EXE_PATH=bin\Debug\net8.0-windows\ProcessManagerApp.exe
    ) else if exist "bin\Debug\net8.0\ProcessManagerApp.exe" (
        set EXE_PATH=bin\Debug\net8.0\ProcessManagerApp.exe
    ) else (
        echo ERROR: Could not find ProcessManagerApp.exe in any bin folder
        echo Please build the project first or check the output path
        pause
        exit /b 1
    )
)

echo Found executable at: %EXE_PATH%
echo.
echo Launching ProcessManagerApp...
echo.

start "" "%EXE_PATH%"

echo.
echo ========================================
echo Application launched!
echo You can close this window.
echo ========================================
pause
