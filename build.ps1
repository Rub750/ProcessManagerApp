#!/usr/bin/env powershell
# Build and publish script for ProcessManagerApp
# This script will compile the application and create a standalone executable

param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "publish",
    [switch]$SelfContained = $true,
    [switch]$NoBuild = $false,
    [switch]$NoRestore = $false
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  ProcessManagerApp - Build & Publish" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "ProcessManagerApp.sln")) {
    Write-Host "ERROR: ProcessManagerApp.sln not found!" -ForegroundColor Red
    Write-Host "Please run this script from the project root directory."
    exit 1
}

# Check for dotnet
$dotnetPath = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnetPath) {
    Write-Host "ERROR: .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8.0+ SDK from: https://dotnet.microsoft.com/download"
    exit 1
}

# Check .NET version
$dotnetVersion = dotnet --version
$majorVersion = ($dotnetVersion -split '\.')[0]

if ($majorVersion -lt 8) {
    Write-Host "ERROR: .NET 8.0 or higher required (found: $dotnetVersion)" -ForegroundColor Red
    exit 1
}

Write-Host ".NET version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Restore packages if needed
if (-not $NoRestore) {
    Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
    dotnet restore ProcessManagerApp.sln
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to restore packages" -ForegroundColor Red
        exit 1
    }
    Write-Host "Packages restored successfully." -ForegroundColor Green
    Write-Host ""
}

# Build the project if needed
if (-not $NoBuild) {
    Write-Host "Building project (Configuration: $Configuration)..." -ForegroundColor Yellow
    dotnet build ProcessManagerApp.sln --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "Build completed successfully." -ForegroundColor Green
    Write-Host ""
}

# Publish the application
$publishArgs = @(
    "publish",
    "ProcessManagerApp.csproj",
    "--configuration", $Configuration,
    "--output", $OutputDir,
    "--no-build"
)

if ($SelfContained) {
    $publishArgs += @("--self-contained", "true")
    $publishArgs += @("--runtime", "win-x64")
}

Write-Host "Publishing application..." -ForegroundColor Yellow
Write-Host "  Configuration: $Configuration" -ForegroundColor Gray
Write-Host "  Output: $OutputDir" -ForegroundColor Gray
Write-Host "  Self-contained: $SelfContained" -ForegroundColor Gray
Write-Host ""

 dotnet $publishArgs
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Publish failed" -ForegroundColor Red
    exit 1
}

Write-Host "Publish completed successfully!" -ForegroundColor Green
Write-Host ""

# Check if executable exists
$exePath = Join-Path $OutputDir "ProcessManagerApp.exe"
if (Test-Path $exePath) {
    Write-Host "Executable created at: $exePath" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run the application by double-clicking:" -ForegroundColor Cyan
    Write-Host "  $exePath" -ForegroundColor Yellow
    Write-Host ""
    
    # Create a simple launch script in the publish directory
    $launchScript = Join-Path $OutputDir "run.bat"
    @"
@echo off
chcp 65001 >nul 2>&1
echo Launching ProcessManagerApp...
start "" "ProcessManagerApp.exe"
"@ | Out-File -FilePath $launchScript -Encoding UTF8
    
    Write-Host "Created launch script: $launchScript" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "WARNING: Executable not found at expected location" -ForegroundColor Yellow
    Write-Host "Please check the publish output directory: $OutputDir" -ForegroundColor Gray
}

# Show directory contents
Write-Host "Publish directory contents:" -ForegroundColor Cyan
Get-ChildItem $OutputDir | Format-Table Name, Length

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  Build & Publish Complete!" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
