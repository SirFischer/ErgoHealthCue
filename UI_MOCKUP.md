# ErgoHealthCue - UI Mockup and Screenshots Description

Since this application is designed for Windows and cannot be run in a Linux environment, this document describes what the application looks like when running.

## System Tray Icon

**Location**: Windows System Tray (bottom-right corner near clock)

**Appearance**: 
- Circular blue icon with white letter "E" in the center
- Icon is 32x32 pixels
- Always visible when application is running

**Interaction**:
- Right-click: Opens context menu
- Double-click: Opens Settings window

**Context Menu**:
```
┌──────────────┐
│ Settings     │
│ Statistics   │
├──────────────┤
│ Exit         │
└──────────────┘
```

---

## Settings Window

**Window Title**: "ErgoHealthCue Settings"
**Size**: 800px × 600px
**Position**: Center of screen

```
╔═══════════════════════════════════════════════════════════════════════╗
║ ErgoHealthCue Settings                                            [×] ║
╠═══════════════════════════════════════════════════════════════════════╣
║                                                                        ║
║  ┌─ General Settings ──────────────────────────────────────────────┐  ║
║  │                                                                  │  ║
║  │  ☑ Use Random Intervals                                         │  ║
║  │                                                                  │  ║
║  │  Minimum Interval (minutes): [  30  ]                           │  ║
║  │  Maximum Interval (minutes): [  60  ]                           │  ║
║  │                                                                  │  ║
║  │  ☐ Start with Windows                                           │  ║
║  │                                                                  │  ║
║  └──────────────────────────────────────────────────────────────────┘  ║
║                                                                        ║
║  ┌─ Quick Actions ─────────────────────────────────────────────────┐  ║
║  │  [ Add New Cue ]  [ Remove Selected ]                           │  ║
║  └──────────────────────────────────────────────────────────────────┘  ║
║                                                                        ║
║  ┌─ Cues ──────────────────────────────────────────────────────────┐  ║
║  │ Enabled │ Type          │ Title          │ Description           │  ║
║  ├─────────┼───────────────┼────────────────┼──────────────────────┤  ║
║  │   ☑     │ DeskStanding  │ Stand Up       │ Raise your desk...   │  ║
║  │   ☑     │ DeskSitting   │ Sit Down       │ Lower your desk...   │  ║
║  │   ☑     │ DeskFloor     │ Floor Position │ Lower your desk...   │  ║
║  │   ☑     │ Stretch       │ Neck Stretch   │ Gently tilt your...  │  ║
║  │   ☑     │ Stretch       │ Shoulder Rolls │ Roll your shoulders..│  ║
║  │   ☑     │ MobilityDrill │ Hip Circles    │ Stand and make...    │  ║
║  │   ☑     │ MobilityDrill │ Ankle Mobility │ Rotate each ankle... │  ║
║  └──────────────────────────────────────────────────────────────────┘  ║
║                                                                        ║
║                          [ View Statistics ]  [ Save ]  [ Cancel ]    ║
╚═══════════════════════════════════════════════════════════════════════╝

Button Colors:
- Save: Green background (#4CAF50), white text, bold
- Cancel: Default gray background
- View Statistics: Default background
```

---

## Cue Overlay Window

**Size**: 500px × 300px
**Position**: Center of screen
**Style**: Transparent background, no title bar, always on top

```
        ╔═══════════════════════════════════════════════╗
        ║                                               ║
        ║              🔷 Stand Up 🔷                   ║
        ║                                               ║
        ║                                               ║
        ║         Raise your desk to standing          ║
        ║                  position                     ║
        ║                                               ║
        ║                                               ║
        ║                                               ║
        ║    ┌─────────────┐      ┌─────────────┐     ║
        ║    │ ✓ Complete  │      │  ✕ Dismiss  │     ║
        ║    └─────────────┘      └─────────────┘     ║
        ║                                               ║
        ╚═══════════════════════════════════════════════╝

Visual Effects:
- Border alternates between Blue (#2196F3) and Orange (#FF9800) every 500ms
- Drop shadow effect around the window
- Background: Light gray (#F5F5F5)
- Title: 24pt, Bold, Blue
- Description: 16pt, Centered, Dark gray
- Complete Button: Green (#4CAF50), white text, rounded corners
- Dismiss Button: Red (#F44336), white text, rounded corners
- Fade-in animation on appearance (300ms)
```

---

## Add Cue Window

**Window Title**: "Add New Cue"
**Size**: 450px × 300px
**Position**: Center over Settings window

```
╔══════════════════════════════════════════╗
║ Add New Cue                          [×] ║
╠══════════════════════════════════════════╣
║                                          ║
║  Cue Type:                               ║
║  ┌────────────────────────────────────┐  ║
║  │ DeskStanding                     ▼ │  ║
║  └────────────────────────────────────┘  ║
║                                          ║
║  Title:                                  ║
║  ┌────────────────────────────────────┐  ║
║  │                                    │  ║
║  └────────────────────────────────────┘  ║
║                                          ║
║  Description:                            ║
║  ┌────────────────────────────────────┐  ║
║  │                                    │  ║
║  │                                    │  ║
║  │                                    │  ║
║  └────────────────────────────────────┘  ║
║                                          ║
║                    [ Add ]  [ Cancel ]   ║
╚══════════════════════════════════════════╝

Dropdown Options for Cue Type:
- DeskStanding
- DeskSitting
- DeskFloor
- Stretch
- MobilityDrill
```

---

## Statistics Window

**Window Title**: "Statistics"
**Size**: 800px × 500px
**Position**: Center over Settings window

```
╔═══════════════════════════════════════════════════════════════════════╗
║ Statistics                                                        [×] ║
╠═══════════════════════════════════════════════════════════════════════╣
║                                                                        ║
║  ┌─ Summary ───────────────────────────────────────────────────────┐  ║
║  │                                                                  │  ║
║  │     Total Cues          Completed           Dismissed           │  ║
║  │         42                  35                  7               │  ║
║  │                          (Green)            (Red)               │  ║
║  └──────────────────────────────────────────────────────────────────┘  ║
║                                                                        ║
║  ┌─ History ───────────────────────────────────────────────────────┐  ║
║  │ Date/Time         │ Type    │ Title       │ Status    │ Time    │  ║
║  ├───────────────────┼─────────┼─────────────┼───────────┼─────────┤  ║
║  │ 2024-01-15 14:30  │ Stretch │ Neck Str... │ Completed │ 15 sec  │  ║
║  │ 2024-01-15 13:45  │ DeskS.. │ Stand Up    │ Completed │ 8 sec   │  ║
║  │ 2024-01-15 12:50  │ Mobil.. │ Hip Circ... │ Dismissed │ 3 sec   │  ║
║  │ 2024-01-15 11:55  │ Stretch │ Shoulder... │ Completed │ 12 sec  │  ║
║  │ 2024-01-15 11:20  │ DeskS.. │ Sit Down    │ Completed │ 5 sec   │  ║
║  └──────────────────────────────────────────────────────────────────┘  ║
║                                                                        ║
║                                                         [ Close ]      ║
╚═══════════════════════════════════════════════════════════════════════╝

Summary Numbers:
- Total: Regular black text, 24pt
- Completed: Green (#4CAF50), 24pt
- Dismissed: Red (#F44336), 24pt
```

---

## Application Flow

```
┌──────────────┐
│   Windows    │
│    Startup   │ (if enabled)
└──────┬───────┘
       │
       v
┌──────────────┐
│ Application  │
│   Starts     │
└──────┬───────┘
       │
       v
┌──────────────┐    ┌─────────────┐
│ System Tray  │◄───┤  Scheduler  │
│     Icon     │    │   Starts    │
└──────┬───────┘    └──────┬──────┘
       │                   │
       │ Right-click       │ Timer expires
       v                   │
┌──────────────┐           v
│  Context     │    ┌─────────────┐
│    Menu      │    │  Cue Popup  │
└──────┬───────┘    │  (Flashing) │
       │            └──────┬──────┘
       │                   │
       │                   │ User Response
       │                   v
       │            ┌─────────────┐
       │            │  Statistics │
       │            │   Updated   │
       │            └──────┬──────┘
       │                   │
       │                   │ Schedule Next
       │                   v
       │            ┌─────────────┐
       │            │  Wait for   │
       │            │   Interval  │
       v            └─────────────┘
┌──────────────┐
│   Settings   │
│   Window     │
└──────────────┘
```

---

## Color Palette

The application uses Material Design colors:

- **Primary Blue**: #2196F3 - Used for titles, borders (cue window)
- **Success Green**: #4CAF50 - Complete button, completed statistics
- **Error Red**: #F44336 - Dismiss button, dismissed statistics
- **Warning Orange**: #FF9800 - Alternating border flash on cue window
- **Background**: #F5F5F5 - Light gray for window backgrounds
- **Text**: #333333 - Dark gray for body text

---

## User Experience Flow

1. **First Launch**:
   - App starts
   - Blue icon appears in system tray
   - Default settings are loaded (30-60 min intervals, 7 default cues)
   - First cue scheduled

2. **Receiving a Cue**:
   - Timer expires (e.g., 45 minutes)
   - Cue overlay appears in center of screen
   - Border flashes blue/orange
   - User must click Complete or Dismiss
   - Window closes, statistics updated
   - Next cue scheduled

3. **Configuring Settings**:
   - Right-click tray icon → Settings
   - Modify intervals, enable/disable cues
   - Add custom cues
   - Enable Windows startup
   - Click Save
   - Scheduler updates with new settings

4. **Viewing Statistics**:
   - Open from tray menu or settings window
   - See summary statistics
   - Browse history of all cues
   - Close when done

This mockup document helps visualize the application without needing to run it on Windows.
