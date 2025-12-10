# GitHub Copilot Instructions for ErgoHealthCue

## Project Overview

ErgoHealthCue is a Windows desktop application built with .NET 8.0 and WPF that promotes healthy ergonomic habits by providing periodic reminders for desk position adjustments and exercises. The application runs as a system tray application with overlay notifications.

## Technology Stack

- **Framework**: .NET 8.0 (net8.0-windows)
- **UI Framework**: WPF (Windows Presentation Foundation) with XAML
- **System Integration**: Windows Forms (NotifyIcon), Registry for auto-start
- **Data Persistence**: JSON with System.Text.Json
- **External Services**: Firebase Realtime Database (FirebaseDatabase.net v4.2.0) for leaderboard
- **Build Tools**: MSI installer using WiX Toolset v5

## Repository Structure

```
ErgoHealthCue/
├── ErgoHealthCue/              # Main application project
│   ├── Models/                 # Data models and enums
│   │   ├── Cue.cs             # Cue definition with Title, Description, Type, IsEnabled
│   │   ├── CueType.cs         # Enum: DeskStanding, DeskSitting, DeskFloor, Stretch, MobilityDrill
│   │   ├── AppSettings.cs     # Application settings and cue list
│   │   ├── CueStatistic.cs    # Statistics tracking model
│   │   ├── UserProgress.cs    # XP, level, and badge tracking
│   │   └── LeaderboardEntry.cs # Global leaderboard data
│   ├── Services/              # Business logic services
│   │   ├── DataService.cs     # JSON persistence to %APPDATA%\ErgoHealthCue\
│   │   ├── CueScheduler.cs    # Timer-based cue scheduling logic
│   │   ├── StartupService.cs  # Windows startup registry integration
│   │   ├── LeaderboardService.cs # Firebase Realtime Database integration
│   │   └── XPCalculator.cs    # XP and level progression logic
│   ├── Windows/               # WPF windows and dialogs
│   │   ├── SettingsWindow.xaml      # Main settings UI (800x600)
│   │   ├── CueOverlayWindow.xaml    # Fullscreen flashing popup
│   │   ├── AddCueWindow.xaml        # Dialog for adding custom cues
│   │   ├── StatisticsWindow.xaml    # Statistics viewer
│   │   ├── LeaderboardWindow.xaml   # Global leaderboard viewer
│   │   ├── WelcomeWindow.xaml       # First-run welcome screen
│   │   ├── UsernamePromptWindow.xaml # Username setup
│   │   ├── LevelUpWindow.xaml       # Level up celebration
│   │   ├── BadgeUnlockedWindow.xaml # Badge unlock notification
│   │   └── PauseDurationWindow.xaml # Pause scheduler dialog
│   ├── App.xaml.cs            # Main application entry point and system tray
│   └── ErgoHealthCue.csproj   # Project file with version and dependencies
├── Installer/                 # MSI installer build scripts
│   ├── BuildInstaller.ps1     # Automated installer build script
│   └── GenerateWixFile.ps1    # WiX file generation
├── BUILDING.md                # Build and installer creation guide
├── DESIGN.md                  # UI flow and application design documentation
├── FIREBASE_SETUP.md          # Firebase configuration instructions
└── README.md                  # User-facing documentation

Data Storage: %APPDATA%\ErgoHealthCue\
├── settings.json              # App configuration and cue list
└── statistics.json            # Historical statistics
```

## Build and Test Instructions

### Building the Application

```bash
# Build in Release mode
dotnet build ErgoHealthCue/ErgoHealthCue.csproj --configuration Release

# Run the application (for testing)
dotnet run --project ErgoHealthCue/ErgoHealthCue.csproj
```

### Publishing

```bash
# Self-contained application
dotnet publish ErgoHealthCue/ErgoHealthCue.csproj -c Release -r win-x64 --self-contained true
```

### Building MSI Installer

```powershell
cd Installer
.\BuildInstaller.ps1 -Version "1.0.0"
```

### Testing

- Currently, there are no automated unit tests in this repository
- Manual testing should be performed by running the application and verifying:
  - System tray integration works
  - Settings can be saved and loaded
  - Cue overlays appear and can be dismissed
  - Statistics are tracked correctly
  - Leaderboard integration (if Firebase is configured)

## Coding Conventions and Best Practices

### C# Code Style

1. **Naming Conventions**:
   - PascalCase for classes, methods, properties, and public fields
   - camelCase for private fields and local variables
   - Prefix private fields with underscore (e.g., `_dataService`)
   - Use descriptive names that clearly indicate purpose

2. **Nullability**:
   - Project has nullable reference types enabled (`<Nullable>enable</Nullable>`)
   - Use nullable annotations appropriately (`?` for nullable types)
   - Handle null cases explicitly

3. **Implicit Usings**:
   - Project uses implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
   - Common namespaces are automatically available

### WPF and XAML Conventions

1. **XAML Structure**:
   - Use data binding where possible (`{Binding PropertyName}`)
   - Follow MVVM patterns loosely (code-behind is acceptable for UI-heavy logic)
   - Use consistent spacing and indentation

2. **Window Sizing**:
   - Settings window: 800x600 pixels
   - Overlay windows: Fullscreen with WindowState="Maximized"
   - Dialog windows: Sized appropriately for content

### Data Persistence

1. **JSON Storage**:
   - Use `System.Text.Json` for serialization
   - Store data in `%APPDATA%\ErgoHealthCue\`
   - File names: `settings.json`, `statistics.json`
   - Handle file I/O errors gracefully

2. **Default Data**:
   - Create default cues on first run (7 default cues as listed in DESIGN.md)
   - Initialize with sensible default intervals (30-60 minutes)

### Firebase Integration

1. **Optional Feature**:
   - Leaderboard is optional and requires Firebase setup
   - Application should work without Firebase configured
   - Handle Firebase errors gracefully (network issues, configuration missing)

2. **Security**:
   - Never commit Firebase credentials or API keys to the repository
   - Use environment variables or secure configuration

## Common Development Tasks

### Adding a New Cue Type

1. Update `CueType.cs` enum with new type
2. Update UI to display new type in dropdowns
3. Update default cue initialization if needed
4. Test cue creation and scheduling

### Adding a New Window/Dialog

1. Create XAML file in `Windows/` directory
2. Create corresponding `.xaml.cs` code-behind
3. Follow existing window patterns for styling and layout
4. Register window in App.xaml if needed for resources

### Modifying Data Models

1. Update model class in `Models/` directory
2. Consider data migration for existing users
3. Update JSON serialization/deserialization if needed
4. Test with existing data files

### Updating Version

1. Update version in `ErgoHealthCue/ErgoHealthCue.csproj`:
   - `<Version>X.Y.Z</Version>`
   - `<AssemblyVersion>X.Y.Z.0</AssemblyVersion>`
   - `<FileVersion>X.Y.Z.0</FileVersion>`
2. Use the same version when building installer

## Dependencies

- **FirebaseDatabase.net** (v4.2.0): Used for global leaderboard feature
  - Only dependency beyond .NET standard libraries
  - Check for security vulnerabilities before updating

## Important Notes

1. **Windows-Only**: This is a Windows-specific application (WPF, Windows Forms components)
2. **System Tray**: Application lifecycle is managed through system tray, not main window
3. **User Data**: Respects user data location in %APPDATA%
4. **Auto-Start**: Uses Windows Registry for startup integration
5. **No Tests**: Currently no automated test suite exists

## Debugging Tips

1. Check application data folder: `%APPDATA%\ErgoHealthCue\`
2. Verify JSON files are valid and readable
3. Check Windows Event Viewer for .NET application errors
4. Test system tray integration by ensuring app stays running after closing windows
5. Verify registry key for auto-start: `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

## Documentation to Reference

- **README.md**: User-facing installation and usage guide
- **BUILDING.md**: Build and installer creation process
- **DESIGN.md**: Detailed UI flow and window specifications
- **FIREBASE_SETUP.md**: Firebase configuration instructions
- **IMPLEMENTATION_SUMMARY.md**: Implementation details and features

## Task Guidance

**Good Tasks for Copilot**:
- Adding new cue types or default cues
- Improving UI layout and styling
- Refactoring existing functionality
- Adding input validation
- Documentation updates
- Bug fixes with clear reproduction steps
- Adding new statistics or tracking features

**Tasks Requiring Human Review**:
- Firebase security rules and configuration
- Windows Registry modifications
- Installer package changes
- Major architectural changes
- Data migration logic

## Questions or Clarifications

When working on tasks:
- Reference existing code patterns in similar files
- Maintain consistency with current architecture
- Consider backward compatibility with existing user data
- Test changes manually before completing tasks
- Ask for clarification if requirements are ambiguous
