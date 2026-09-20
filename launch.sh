#!/bin/bash

echo "========================================"
echo "  ProcessManagerApp - Lancement"
echo "========================================"
echo ""

# Vérifier si dotnet est installé
if ! command -v dotnet &> /dev/null; then
    echo "ERREUR: .NET SDK n'est pas installé ou n'est pas dans le PATH"
    echo "Téléchargez .NET 8.0 SDK depuis: https://dotnet.microsoft.com/download"
    read -p "Appuyez sur Entrée pour quitter..."
    exit 1
fi

# Vérifier la version de dotnet
DOTNET_VERSION=$(dotnet --version)
MAJOR_VERSION=$(echo "$DOTNET_VERSION" | grep -oE '^[0-9]+')

if [ "$MAJOR_VERSION" -lt 8 ]; then
    echo "ERREUR: .NET 8.0 ou supérieur est requis (version détectée: $DOTNET_VERSION)"
    read -p "Appuyez sur Entrée pour quitter..."
    exit 1
fi

echo "Version de .NET détectée: $DOTNET_VERSION"
echo ""

# Restaurer les packages NuGet
echo "Restauration des packages NuGet..."
dotnet restore ProcessManagerApp.sln
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la restauration des packages"
    read -p "Appuyez sur Entrée pour quitter..."
    exit 1
fi

# Builder le projet en Release
echo "Construction du projet..."
dotnet build ProcessManagerApp.sln --configuration Release --no-restore
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la compilation"
    read -p "Appuyez sur Entrée pour quitter..."
    exit 1
fi

# Trouver l'executable
EXE_PATH="bin/Release/net8.0-windows/ProcessManagerApp"
if [ ! -f "$EXE_PATH" ]; then
    echo "ERREUR: Exécutable introuvable: $EXE_PATH"
    read -p "Appuyez sur Entrée pour quitter..."
    exit 1
fi

echo ""
echo "Lancement de ProcessManagerApp..."
echo ""

# Lancer l'application
dotnet "$EXE_PATH.dll"

echo ""
echo "Application terminée."
