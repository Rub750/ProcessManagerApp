#!/bin/bash

# Build script for Process Manager Pro
CONFIGURATION="Release"
OUTPUT_PATH="./bin/Release/net8.0-windows/win-x64/publish"
SELF_CONTAINED=true

while [[ "$#" -gt 0 ]]; do
    case $1 in
        --clean|-c) CLEAN=true ;;
        --debug|-d) CONFIGURATION="Debug" ;;
        --output|-o) OUTPUT_PATH="$2" ; shift ;;
        --no-self-contained) SELF_CONTAINED=false ;;
        *) echo "Unknown parameter: $1" ; exit 1 ;;
    esac
    shift
done

echo "Building Process Manager Pro..."

# Clean if requested
if [ "$CLEAN" = true ]; then
    echo "Cleaning solution..."
    dotnet clean ProcessManagerApp.sln -c $CONFIGURATION
fi

# Restore packages
echo "Restoring NuGet packages..."
dotnet restore ProcessManagerApp.sln || { echo "Failed to restore NuGet packages"; exit 1; }

# Build the project
echo "Building project..."
dotnet build ProcessManagerApp.csproj -c $CONFIGURATION --no-restore || { echo "Failed to build project"; exit 1; }

# Publish the application
echo "Publishing application..."
dotnet publish ProcessManagerApp.csproj \
    -c $CONFIGURATION \
    -r win-x64 \
    --self-contained $SELF_CONTAINED \
    -p:PublishSingleFile=true \
    -p:PublishReadyToRun=true \
    -p:PublishTrimmed=true \
    -o $OUTPUT_PATH || { echo "Failed to publish application"; exit 1; }

echo "Build completed successfully!"
echo "Output directory: $OUTPUT_PATH"

# Check if output exists
if [ -d "$OUTPUT_PATH" ]; then
    echo "Application files:"
    ls -lh "$OUTPUT_PATH"
fi
