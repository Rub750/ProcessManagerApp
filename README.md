# Process Manager Pro

Application Windows avancée pour le monitoring en temps réel des ressources système et la gestion des processus.

## Fonctionnalités

- **Surveillance en temps réel** : CPU, RAM, disques, réseau
- **Gestion des processus** : Affichage, tri, filtrage, arrêt
- **Optimisation du CPU** : Modes Gaming, Productivité, Économie de batterie
- **Interface professionnelle** : Design moderne et intuitif
- **Notifications** : Alertes pour les actions importantes
- **Historique d'utilisation** : Graphiques et historiques des ressources

## Prérequis

- Windows 10 ou supérieur (x64)
- Rien d'autre à installer

## Installation (sans rien installer)

1. Aller sur la page [Releases](https://github.com/Rub750/ProcessManagerApp/releases)
2. Télécharger `ProcessManagerApp-win-x64.zip`
3. Extraire le zip
4. Exécuter `ProcessManagerApp.exe`

Le zip contient une version autonome (self-contained) : le runtime .NET 8 est intégré dans l'exécutable, aucun安装 de .NET n'est nécessaire.

## Installation (mode développeur)

1. Cloner le dépôt ou télécharger les fichiers
2. Exécuter `launch.bat` pour construire et lancer l'application (nécessite le SDK .NET 8.0)
3. Ou construire manuellement avec Visual Studio 2022

## Build manuel

```bash
# PowerShell
dotnet restore
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Ou utiliser le script
./build.ps1
```

## Utilisation

### Lancement
- Exécuter `ProcessManagerApp.exe`
- L'application nécessite des droits administrateur pour certaines fonctionnalités

### Fonctionnalités principales

1. **Tableau de bord** : Vue d'ensemble des ressources système
2. **Liste des processus** : Tous les processus en cours avec détails
3. **Optimisation** :
   - Mode Gaming : Optimise pour les jeux vidéo
   - Mode Productivité : Optimise pour le travail
   - Mode Économie : Réduit la consommation d'énergie
4. **Actions sur les processus** :
   - Arrêter un processus
   - Fermer une tâche proprement
   - Suspendre/Reprendre un processus

### Raccourcis clavier

- `F5` : Rafraîchir les données
- `Échap` : Quitter le mode plein écran
- Double-clic sur la barre de titre : Basculer plein écran

## Architecture

L'application utilise :
- **WPF** pour l'interface graphique
- **MVVM** comme pattern de conception
- **PerformanceCounter** pour le monitoring système
- **Process** pour la gestion des processus

## Structure du projet

```
ProcessManagerApp/
├── Models/           # Classes de données et logique métier
├── ViewModels/       # ViewModels pour le binding MVVM
├── Views/            # Vues XAML
├── Resources/        # Ressources (styles, couleurs, images)
├── Converters/       # Converters pour le binding
├── Properties/       # Configuration et paramètres
├── App.xaml          # Point d'entrée de l'application
└── build.ps1         # Script de build
```

## Contribution

Les contributions sont les bienvenues ! Ouvrir une Pull Request avec vos améliorations.

## Licence

MIT License

## Auteur

ProcessManagerApp Team
