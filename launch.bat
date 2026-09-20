@echo off
chcp 65001 >nul 2>&1

echo ========================================
echo   ProcessManagerApp - Lancement
echo ========================================
echo.

REM Vérifier si dotnet est installé
where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERREUR: .NET SDK n'est pas installé ou n'est pas dans le PATH
    echo Telechargez .NET 8.0 SDK depuis: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Vérifier la version de dotnet
for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
if "%DOTNET_VERSION%" LSS "8.0" (
    echo ERREUR: .NET 8.0 ou superieur est requis (version detectee: %DOTNET_VERSION%)
    pause
    exit /b 1
)

echo Version de .NET detectee: %DOTNET_VERSION%
echo.

REM Restaurer les packages NuGet
echo Restauration des packages NuGet...
dotnet restore ProcessManagerApp.sln
if %ERRORLEVEL% neq 0 (
    echo ERREUR: Echec de la restauration des packages
    pause
    exit /b 1
)

REM Builder le projet en Release
echo Construction du projet...
dotnet build ProcessManagerApp.sln --configuration Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo ERREUR: Echec de la compilation
    pause
    exit /b 1
)

REM Trouver l'executable
set EXE_PATH=bin\Release\net8.0-windows\ProcessManagerApp.exe
if not exist "%EXE_PATH%" (
    echo ERREUR: Executable introuvable: %EXE_PATH%
    pause
    exit /b 1
)

echo.
echo Lancement de ProcessManagerApp...
echo.

REM Lancer l'application
start "" "%EXE_PATH%"

echo Application lancee avec succes!
echo Vous pouvez fermer cette fenetre.
