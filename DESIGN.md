# ErgoHealthCue - Application Flow and UI Description

## System Tray Application

The application runs in the Windows system tray with a blue circular icon containing the letter "E".

### Tray Menu Options:
- **Settings** - Opens the settings window
- **Statistics** - Opens the statistics window
- **Exit** - Closes the application

Double-clicking the tray icon also opens the Settings window.

## Windows and UI Components

### 1. Settings Window (SettingsWindow.xaml)
**Dimensions**: 800x600 pixels

**Sections**:

#### General Settings
- **Use Random Intervals** checkbox - Toggle between random and fixed intervals
- **Minimum Interval (minutes)** - Text input for minimum wait time (default: 30)
- **Maximum Interval (minutes)** - Text input for maximum wait time (default: 60)
- **Start with Windows** checkbox - Enable/disable auto-start

#### Quick Actions
- **Add New Cue** button - Opens dialog to create custom cues
- **Remove Selected** button - Removes the selected cue from the list

#### Cues List (DataGrid)
Displays all available cues with columns:
- **Enabled** - Checkbox to enable/disable each cue
- **Type** - CueType enum (DeskStanding, DeskSitting, DeskFloor, Stretch, MobilityDrill)
- **Title** - Cue title (editable)
- **Description** - Detailed instructions (editable)

#### Default Cues:
1. **Stand Up** (DeskStanding) - "Raise your desk to standing position"
2. **Sit Down** (DeskSitting) - "Lower your desk to sitting position"
3. **Floor Position** (DeskFloor) - "Lower your desk all the way down"
4. **Neck Stretch** (Stretch) - "Gently tilt your head to each side, holding for 10 seconds"
5. **Shoulder Rolls** (Stretch) - "Roll your shoulders backwards 10 times"
6. **Hip Circles** (MobilityDrill) - "Stand and make large circles with your hips, 10 each direction"
7. **Ankle Mobility** (MobilityDrill) - "Rotate each ankle 10 times in each direction"

#### Bottom Buttons
- **View Statistics** - Opens the statistics window
- **Save** (green) - Saves settings and applies changes
- **Cancel** - Closes without saving

---

### 2. Add Cue Window (AddCueWindow.xaml)
**Dimensions**: 450x300 pixels

**Fields**:
- **Cue Type** dropdown - Select from available cue types
- **Title** text box - Enter cue title
- **Description** multi-line text box - Enter detailed instructions

**Buttons**:
- **Add** - Creates the new cue
- **Cancel** - Closes without adding

---

### 3. Cue Overlay Window (CueOverlayWindow.xaml)
**Dimensions**: 500x300 pixels
**Style**: Transparent, borderless, always on top, centered on screen

**Visual Design**:
- Rounded border with alternating colors (Blue #2196F3 / Orange #FF9800)
- Flashing effect at 500ms intervals
- Drop shadow for prominence
- Light gray background (#F5F5F5)

**Content**:
- **Title** - Large blue text (24pt, bold)
- **Description** - Centered instruction text (16pt)
- **Complete Button** (green, ✓) - Marks cue as completed and logs to statistics
- **Dismiss Button** (red, ✕) - Dismisses cue and logs as incomplete

**Behavior**:
- Appears at scheduled intervals (random or fixed)
- Border flashes between blue and orange until user responds
- Fades in with 300ms animation
- Cannot be closed without clicking a button

---

### 4. Statistics Window (StatisticsWindow.xaml)
**Dimensions**: 900x600 pixels

**Summary Section**:
Five columns displaying:
- **Level** - Current user level (1-100)
- **Progress** - Progress to next level (percentage and XP)
- **Total Cues** - Total number of cues shown
- **Completed** (green) - Number of completed cues
- **Dismissed** (red) - Number of dismissed cues

**Streak and Badges Section**:
Two cards displaying:
- **Current Streak** - Number of consecutive completed cues and best streak record
- **Badges Unlocked** - Count of unlocked badges (out of 113 total)
  - Shows names of unlocked badges
  - **Clickable** - Opens Badge List Window when clicked

**History Section (DataGrid)**:
Shows all cue interactions with columns:
- **Date/Time** - When the cue was shown (YYYY-MM-DD HH:MM format)
- **Type** - CueType enum value
- **Title** - Cue title
- **Status** - "Completed" or "Dismissed"
- **Response Time** - Time taken to respond (seconds/minutes/hours)

**Buttons**:
- **Reset Statistics** - Resets all progress data
- **Close** - Closes the statistics window

---

### 5. Badge List Window (BadgeListWindow.xaml)
**Dimensions**: 800x650 pixels

Opens when clicking on the badge count in the Statistics Window.

**Header**:
- Displays title "🏆 Badge Achievements"
- Shows unlocked badge count (e.g., "5 / 113 badges unlocked")

**Filter Options**:
- **All Badges** - Shows all 113 badges (default)
- **Achievement Badges** - Shows only positive badges (100 total)
- **Negative Badges** - Shows only negative badges (13 total)
- **Unlocked Only** - Shows only earned badges

**Badge Display**:
Each badge is displayed as a card showing:
- **Icon** - 🏆 for unlocked positive badges, 💩 for unlocked negative badges, 🔒 for locked badges
- **Name** - Badge name (colored gold for unlocked positive, red for unlocked negative, grey for locked)
- **Requirement** - Shows requirement for unlocked badges (e.g., "Complete 10 cues in a row"), shows "???" for locked badges

**Badge Types**:

*Positive Achievement Badges (100 total)*:
- Earned by completing cues in a row
- Ranges from "First Step" (1 cue) to "Legendary" (10,000 cues)
- Organized in tiers: Early achievements (1-20), Building momentum (21-40), Consistent effort (41-60), Advanced dedication (61-80), Elite tier (81-100)

*Negative Badges (13 total)*:
- Earned by dismissing cues in a row (humorous/sarcastic)
- Ranges from "Rebel 😏" (3 dismissals) to "Back Pain Collector 💀" (200 dismissals)
- Displayed with poop emoji and red border when unlocked

**Visual Styling**:
- Unlocked positive badges: Gold border with hover effect
- Unlocked negative badges: Red border with hover effect
- Locked badges: Grey background with reduced opacity
- Cards are arranged in a wrap panel for easy browsing

**Button**:
- **Close** - Closes the badge list window

---

## Data Persistence

### Location
All data stored in: `%APPDATA%\ErgoHealthCue\`

### Files

**settings.json** - Contains:
- Interval configuration (min/max minutes, random vs fixed)
- Startup preferences
- All cue definitions

**statistics.json** - Contains:
- Array of all cue interactions
- Timestamps for shown, completed, and dismissed events
- Cue details for each interaction

---

## Scheduler Behavior

1. On application start, scheduler begins immediately
2. Waits for configured interval (30-60 minutes by default)
3. Selects random enabled cue from the list
4. Displays overlay window
5. Logs the event with timestamp
6. Waits for user response (Complete or Dismiss)
7. Updates statistics with response time
8. Schedules next cue

**Random Intervals**: If enabled, each interval is randomly selected between min and max values
**Fixed Intervals**: If disabled, uses minimum interval value consistently

---

## Startup Integration

When "Start with Windows" is enabled:
- Adds registry entry to: `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
- Key name: "ErgoHealthCue"
- Value: Full path to executable

When disabled:
- Removes the registry entry

---

## Technical Stack

- **Framework**: .NET 8.0
- **UI**: WPF (Windows Presentation Foundation)
- **System Integration**: Windows Forms (for NotifyIcon)
- **Data Format**: JSON
- **Target OS**: Windows (net8.0-windows)

---

## Color Scheme

- **Primary Blue**: #2196F3 (Material Design Blue)
- **Success Green**: #4CAF50 (Material Design Green)
- **Error Red**: #F44336 (Material Design Red)
- **Warning Orange**: #FF9800 (Material Design Orange)
- **Background**: #F5F5F5 (Light Gray)
- **Text**: #333333 (Dark Gray)
