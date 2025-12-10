# ErgoHealthCue

ErgoHealthCue is a Windows desktop application designed to promote healthy ergonomic habits by providing periodic reminders (cues) to adjust your desk position and perform stretches and mobility exercises.

## Features

- **System Tray Application**: Runs silently in the background with a system tray icon
- **Customizable Cues**: Add, remove, and manage desk position reminders and exercise cues
  - Desk Standing Position
  - Desk Sitting Position
  - Desk Floor Position
  - Stretch Exercises
  - Mobility Drills
- **Flexible Scheduling**: Configure fixed or random intervals between cues (30-60 minutes by default)
- **Overlay Notifications**: Eye-catching, flashing popup overlays that require acknowledgment
- **Statistics Tracking**: Track completion rates and response times for all cues
- **Global Leaderboard**: Compete with other users worldwide (requires Firebase setup)
  - Compare levels, XP, and completion rates
  - See your global ranking
  - Track completed and dismissed cues
- **Auto-Start**: Optional Windows startup integration

## Quick Start

### Download and Install (Recommended)

1. Download the latest MSI installer from the [Releases](https://github.com/SirFischer/ErgoHealthCue/releases) page
2. Run the installer and follow the installation wizard
3. Launch ErgoHealthCue from the desktop shortcut or Start Menu
4. The application will run in your system tray

### Building from Source

If you prefer to build from source:

#### Prerequisites

- .NET 8.0 SDK or later
- Windows OS (Windows 10 or later recommended)

#### Build and Run

1. Clone the repository:
```bash
git clone https://github.com/SirFischer/ErgoHealthCue.git
cd ErgoHealthCue
```

2. Build the project:
```bash
dotnet build ErgoHealthCue/ErgoHealthCue.csproj --configuration Release
```

3. Run the application:
```bash
dotnet run --project ErgoHealthCue/ErgoHealthCue.csproj
```

Or execute directly:
```bash
ErgoHealthCue/bin/Release/net8.0-windows/ErgoHealthCue.exe
```

### Publishing for Distribution

To create a self-contained executable:

```bash
# Single-file executable
dotnet publish ErgoHealthCue/ErgoHealthCue.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Framework-dependent (requires .NET 8.0 runtime)
dotnet publish ErgoHealthCue/ErgoHealthCue.csproj -c Release -r win-x64
```

The executable will be in `ErgoHealthCue/bin/Release/net8.0-windows/win-x64/publish/`

## Usage

1. **Launch the Application**: After starting, the app runs in the system tray (look for a blue icon with "E")
2. **Access Settings**: Right-click the tray icon and select "Settings"
3. **Configure Cues**: 
   - Enable/disable existing cues
   - Add custom cues using the "Add New Cue" button
   - Remove unwanted cues with "Remove Selected"
4. **Set Intervals**: 
   - Adjust the minimum and maximum interval between cues
   - Toggle "Use Random Intervals" for varied timing
5. **Enable Auto-Start**: Check "Start with Windows" to run the app on system startup
6. **Respond to Cues**: When a cue appears:
   - Click **✓ Complete** if you performed the action
   - Click **✕ Dismiss** to skip it
7. **View Statistics**: 
   - Access from the tray menu or settings window
   - See completion rates and response times

## Application Structure

```
ErgoHealthCue/
├── Models/               # Data models
│   ├── Cue.cs           # Cue definition
│   ├── CueType.cs       # Cue type enumeration
│   ├── AppSettings.cs   # Application settings
│   └── CueStatistic.cs  # Statistics model
├── Services/            # Business logic
│   ├── DataService.cs   # JSON data persistence
│   ├── CueScheduler.cs  # Timer and cue scheduling
│   └── StartupService.cs # Windows startup integration
├── Windows/             # UI components
│   ├── SettingsWindow.xaml      # Settings UI
│   ├── CueOverlayWindow.xaml    # Cue popup UI
│   ├── AddCueWindow.xaml        # Add cue dialog
│   └── StatisticsWindow.xaml    # Statistics viewer
└── App.xaml.cs          # Main application and system tray
```

## Data Storage

Settings and statistics are stored in JSON format at:
```
%APPDATA%\ErgoHealthCue\
├── settings.json      # App configuration and cues
└── statistics.json    # Historical statistics
```

## Default Cues

The application comes with 7 pre-configured cues:

1. **Stand Up** - Raise your desk to standing position
2. **Sit Down** - Lower your desk to sitting position  
3. **Floor Position** - Lower your desk all the way down
4. **Neck Stretch** - Gently tilt your head to each side, holding for 10 seconds
5. **Shoulder Rolls** - Roll your shoulders backwards 10 times
6. **Hip Circles** - Stand and make large circles with your hips, 10 each direction
7. **Ankle Mobility** - Rotate each ankle 10 times in each direction

## Technical Details

- **Framework**: .NET 8.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **System Integration**: Windows Forms (NotifyIcon), Registry
- **Data Format**: JSON with System.Text.Json
- **Target Platform**: Windows (net8.0-windows)

## Troubleshooting

### Application doesn't start
- Ensure .NET 8.0 runtime is installed
- Check Windows Event Viewer for error messages

### Auto-start not working
- Run the application as administrator once
- Check registry key: `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

### Cues not appearing
- Verify cues are enabled in settings
- Check that minimum interval has elapsed
- Ensure application is still running in system tray

## Leaderboard Setup

To enable the global leaderboard feature, you need to set up a Firebase Realtime Database. See [FIREBASE_SETUP.md](FIREBASE_SETUP.md) for detailed instructions.

## Building the Installer

For detailed instructions on building the MSI installer from source, see [BUILDING.md](BUILDING.md).

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open source and available under the MIT License.

## Documentation

For detailed information about the UI and application flow, see [DESIGN.md](DESIGN.md).

