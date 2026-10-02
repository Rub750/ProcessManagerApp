# Build script for Process Manager Pro
param(
    [string]$Configuration = "Release",
    [string]$OutputPath = ".\bin\Release\net8.0-windows\win-x64\publish",
    [switch]$Clean = $false,
    [switch]$SelfContained = $true
)

Write-Host "Building Process Manager Pro..." -ForegroundColor Cyan

# Clean if requested
if ($Clean) {
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    dotnet clean ProcessManagerApp.sln -c $Configuration
}

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
if (-not (dotnet restore ProcessManagerApp.sln)) {
    Write-Error "Failed to restore NuGet packages"
    exit 1
}

# Build the project
Write-Host "Building project..." -ForegroundColor Yellow
if (-not (dotnet build ProcessManagerApp.csproj -c $Configuration --no-restore)) {
    Write-Error "Failed to build project"
    exit 1
}

# Publish the application
Write-Host "Publishing application..." -ForegroundColor Yellow
$publishArgs = @(
    "publish",
    "ProcessManagerApp.csproj",
    "-c", $Configuration,
    "-r", "win-x64",
    "--self-contained", $SelfContained,
    "-p:PublishSingleFile=true",
    "-p:PublishReadyToRun=true",
    "-p:PublishTrimmed=true",
    "-o", $OutputPath
)

if (-not (dotnet @publishArgs)) {
    Write-Error "Failed to publish application"
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Output directory: $OutputPath" -ForegroundColor Green

# Check if output exists
if (Test-Path $OutputPath) {
    Write-Host "Application files:" -ForegroundColor Cyan
    Get-ChildItem $OutputPath | Select-Object Name, Length | Format-Table -AutoSize
}
