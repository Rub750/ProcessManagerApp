#!/bin/bash
# Build and publish script for ProcessManagerApp
# This script will compile the application and create a standalone executable

# Default parameters
CONFIGURATION="Release"
OUTPUT_DIR="publish"
SELF_CONTAINED=true
NO_BUILD=false
NO_RESTORE=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        --configuration|-c)
            CONFIGURATION="$2"
            shift 2
            ;;
        --output|-o)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        --no-self-contained)
            SELF_CONTAINED=false
            shift
            ;;
        --no-build)
            NO_BUILD=true
            shift
            ;;
        --no-restore)
            NO_RESTORE=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo "=========================================="
echo "  ProcessManagerApp - Build & Publish"
echo "=========================================="
echo ""

# Check if we're in the right directory
if [ ! -f "ProcessManagerApp.sln" ]; then
    echo "ERROR: ProcessManagerApp.sln not found!"
    echo "Please run this script from the project root directory."
    exit 1
fi

# Check for dotnet
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found!"
    echo "Please install .NET 8.0+ SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
MAJOR_VERSION=$(echo "$DOTNET_VERSION" | grep -oE '^[0-9]+')

if [ "$MAJOR_VERSION" -lt 8 ]; then
    echo "ERROR: .NET 8.0 or higher required (found: $DOTNET_VERSION)"
    exit 1
fi

echo ".NET version: $DOTNET_VERSION"
echo ""

# Restore packages if needed
if [ "$NO_RESTORE" = false ]; then
    echo "Restoring NuGet packages..."
    dotnet restore ProcessManagerApp.sln
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to restore packages"
        exit 1
    fi
    echo "Packages restored successfully."
    echo ""
fi

# Build the project if needed
if [ "$NO_BUILD" = false ]; then
    echo "Building project (Configuration: $CONFIGURATION)..."
    dotnet build ProcessManagerApp.sln --configuration $CONFIGURATION --no-restore
    if [ $? -ne 0 ]; then
        echo "ERROR: Build failed"
        exit 1
    fi
    echo "Build completed successfully."
    echo ""
fi

# Publish the application
PUBLISH_ARGS=(
    "publish"
    "ProcessManagerApp.csproj"
    "--configuration" "$CONFIGURATION"
    "--output" "$OUTPUT_DIR"
    "--no-build"
)

if [ "$SELF_CONTAINED" = true ]; then
    PUBLISH_ARGS+=("--self-contained" "true")
    PUBLISH_ARGS+=("--runtime" "win-x64")
fi

echo "Publishing application..."
echo "  Configuration: $CONFIGURATION"
echo "  Output: $OUTPUT_DIR"
echo "  Self-contained: $SELF_CONTAINED"
echo ""

dotnet "${PUBLISH_ARGS[@]}"
if [ $? -ne 0 ]; then
    echo "ERROR: Publish failed"
    exit 1
fi

echo "Publish completed successfully!"
echo ""

# Check if executable exists
EXE_PATH="$OUTPUT_DIR/ProcessManagerApp.exe"
if [ -f "$EXE_PATH" ]; then
    echo "Executable created at: $EXE_PATH"
    echo ""
    echo "You can now run the application by double-clicking:"
    echo "  $EXE_PATH"
    echo ""
    
    # Create a simple launch script in the publish directory
    cat > "$OUTPUT_DIR/run.bat" << 'EOF'
@echo off
chcp 65001 >nul 2>&1
echo Launching ProcessManagerApp...
start "" "ProcessManagerApp.exe"
EOF
    
    echo "Created launch script: $OUTPUT_DIR/run.bat"
    echo ""
else
    echo "WARNING: Executable not found at expected location"
    echo "Please check the publish output directory: $OUTPUT_DIR"
fi

# Show directory contents
echo "Publish directory contents:"
ls -lh "$OUTPUT_DIR"

echo ""
echo "=========================================="
echo "  Build & Publish Complete!"
echo "=========================================="
