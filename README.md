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
- **Auto-Start**: Optional Windows startup integration

## Building and Running

### Prerequisites

- .NET 8.0 SDK or later
- Windows OS (Windows 10 or later recommended)

### Build

```bash
dotnet build ErgoHealthCue/ErgoHealthCue.csproj
```

### Run

```bash
dotnet run --project ErgoHealthCue/ErgoHealthCue.csproj
```

Or run the executable directly:

```bash
ErgoHealthCue/bin/Debug/net8.0-windows/ErgoHealthCue.exe
```

## Usage

1. **Launch the Application**: After starting, the app runs in the system tray
2. **Access Settings**: Right-click the tray icon and select "Settings"
3. **Configure Cues**: Add custom cues or enable/disable existing ones
4. **Set Intervals**: Adjust the minimum and maximum interval between cues
5. **Enable Auto-Start**: Check "Start with Windows" to run the app on system startup
6. **Respond to Cues**: When a cue appears:
   - Click **Complete** if you performed the action
   - Click **Dismiss** to skip it
7. **View Statistics**: Access statistics from the tray menu or settings window

## Data Storage

Settings and statistics are stored in JSON format at:
```
%APPDATA%\ErgoHealthCue\settings.json
%APPDATA%\ErgoHealthCue\statistics.json
```

## License

This project is open source and available under the MIT License.
