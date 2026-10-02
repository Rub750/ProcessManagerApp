#!/bin/bash

# Launch script for Process Manager Pro
# This script builds and runs the application

cd "$(dirname "$0")"

echo "Building Process Manager Pro..."
./build.sh --clean

if [ $? -ne 0 ]; then
    echo "Build failed!"
    read -p "Press Enter to exit..."
    exit 1
fi

echo "Launching application..."
if [ -f "./bin/Release/net8.0-windows/win-x64/publish/ProcessManagerApp.exe" ]; then
    wine ./bin/Release/net8.0-windows/win-x64/publish/ProcessManagerApp.exe
else
    echo "Application executable not found!"
    read -p "Press Enter to exit..."
    exit 1
fi

echo "Application launched successfully!"
read -p "Press Enter to exit..."
