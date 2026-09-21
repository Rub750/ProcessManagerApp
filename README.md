# ProcessManagerApp

Un gestionnaire de processus Windows avec interface graphique moderne.

## 📋 Description

ProcessManagerApp est une application WPF (.NET 8.0) qui permet de visualiser et gérer les processus en cours d'exécution sur votre système Windows. L'application offre une interface utilisateur intuitive avec :

- **Liste complète des processus** avec leurs informations détaillées
- **Filtrage par nom, catégorie et priorité**
- **Rafraîchissement automatique** toutes les 2 secondes
- **Interface moderne et responsive**
- **Gestion des erreurs intégrée**

## 🎨 Interface Graphique

L'application dispose déjà d'une interface graphique complète basée sur WPF avec :

- Fenêtre principale avec tableau des processus
- Barre de recherche et filtres
- Styles modernes et couleurs thématiques
- Icônes personnalisées
- Notifications et messages de statut

## 🚀 Installation et Exécution

### Prérequis
- [.NET 8.0 SDK ou Runtime](https://dotnet.microsoft.com/download) (requis pour la compilation)
- Windows 10 ou supérieur

### Méthode 1: Exécution directe (développement)

1. Cloner le dépôt :
   ```bash
   git clone https://github.com/Rub750/ProcessManagerApp.git
   cd ProcessManagerApp
   ```

2. Restaurer les packages et compiler :
   ```bash
   dotnet restore ProcessManagerApp.sln
   dotnet build ProcessManagerApp.sln --configuration Release
   ```

3. Exécuter l'application :
   ```bash
   dotnet run --project ProcessManagerApp.csproj
   ```
   ou double-cliquer sur :
   ```
   bin\Release\net8.0-windows\ProcessManagerApp.exe
   ```

### Méthode 2: Utiliser les scripts fournis

#### Sur Windows (PowerShell) :
```powershell
# Exécuter directement (nécessite .NET Runtime)
.\[launch.bat](launch.bat)

# Ou compiler et publier
.\[build.ps1](build.ps1)
```

#### Sur Linux/macOS (Bash) :
```bash
# Donner les permissions d'exécution
chmod +x launch.sh
chmod +x build.sh

# Exécuter directement (nécessite .NET Runtime)
./launch.sh

# Ou compiler et publier
./build.sh
```

### Méthode 3: Créer un exécutable autonome

Pour créer un exécutable auto-contenu qui ne nécessite pas .NET Runtime :

```bash
# Sur Windows
dotnet publish ProcessManagerApp.csproj --configuration Release --runtime win-x64 --self-contained true --output publish

# Sur Linux (pour Windows)
dotnet publish ProcessManagerApp.csproj --configuration Release --runtime win-x64 --self-contained true --output publish
```

L'exécutable sera généré dans le dossier `publish/`.

## 📦 Contenu du projet

```
ProcessManagerApp/
├── App.xaml              # Point d'entrée de l'application WPF
├── App.xaml.cs          # Logique de démarrage et gestion des erreurs
├── ProcessManagerApp.csproj  # Configuration du projet
├── ProcessManagerApp.sln    # Solution Visual Studio
├── Models/
│   └── ProcessInfo.cs   # Modèle de données pour les processus
├── Services/
│   └── ProcessManager.cs # Service de gestion des processus
├── ViewModels/
│   └── MainViewModel.cs # ViewModel pour la logique métier
├── Views/
│   ├── MainWindow.xaml  # Interface principale
│   ├── MainWindow.xaml.cs
│   ├── Styles.xaml      # Styles globaux
│   └── Converters/      # Converters WPF
├── Resources/
│   ├── process.ico      # Icône de l'application
│   └── warning.ico      # Icône d'avertissement
├── launch.bat           # Script de lancement Windows
├── launch.sh            # Script de lancement Linux/macOS
├── build.ps1            # Script de build PowerShell
└── build.sh             # Script de build Bash
```

## ⚙️ Configuration

### Personnalisation

- **Icône** : Modifiez les fichiers `.ico` dans le dossier `Resources/`
- **Styles** : Éditez `Views/Styles.xaml` pour modifier l'apparence
- **Comportement** : Modifiez `Services/ProcessManager.cs` pour changer la logique de rafraîchissement

### Paramètres de build

Le projet est configuré pour :
- Cibler .NET 8.0
- Utiliser WPF
- Générer un exécutable WinExe
- Inclure l'icône de l'application

## 🔧 Fonctionnalités

### Fonctionnalités actuelles
- ✅ Affichage de tous les processus en cours
- ✅ Filtrage par nom, catégorie et priorité
- ✅ Rafraîchissement automatique
- ✅ Gestion des erreurs
- ✅ Interface graphique moderne
- ✅ Icônes personnalisées

### Fonctionnalités futures
- [ ] Arrêt/Redémarrage des processus
- [ ] Modification de la priorité
- [ ] Graphiques d'utilisation CPU/RAM
- [ ] Export des données

## 🛠️ Technologies utilisées

- **.NET 8.0** - Framework principal
- **WPF** - Interface graphique
- **MVVM** - Pattern de conception
- **System.Diagnostics** - Gestion des processus

## 📝 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

## 🤝 Contribution

Les contributions sont les bienvenues ! Veuillez ouvrir une Pull Request avec vos modifications.

## 📞 Support

Pour toute question ou problème, ouvrez une issue sur GitHub.

---

**Note** : Cette application est conçue pour Windows. Elle peut fonctionner sur d'autres plateformes avec WINE ou Mono, mais cela n'est pas officiellement supporté.
