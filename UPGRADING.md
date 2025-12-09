# Upgrading ErgoHealthCue

## Loading New Default Cues

If you're upgrading from a previous version, you may not see the new comprehensive exercise library (73 default cues). Here's how to add them:

### Method 1: Load Default Cues Button (Recommended)

1. Open the application from the system tray
2. Right-click the ErgoHealthCue icon and select **Settings**
3. Scroll down to the **Cues** section
4. Click the green **"Load Default Cues"** button
5. Confirm the action
6. The app will add all default cues that you don't already have

### Method 2: Reset Settings (Complete Reset)

If you want to start fresh with all default settings and cues:

1. Close the ErgoHealthCue application completely
2. Navigate to `%APPDATA%\ErgoHealthCue\`
3. Delete the `settings.json` file
4. Restart the application

**Note:** This will reset ALL your settings including intervals and customizations.

## New Default Cues Library (73 Total)

### Position Changes (3)
- Standing, Sitting, Floor desk adjustments

### Standing Position (14 exercises)
- **Stretches:** Neck, shoulders, calves, quads, side bends, forward folds, chest openers
- **Mobility:** Hip circles, ankle mobility, leg swings, torso twists, marching, arm circles, hip flexor kicks

### Sitting Position (20 exercises)
- **Stretches:** Neck variations, upper traps, chest, torso twists, forward/side bends, figure-4 hip stretch, hip flexor stretch, glute stretch, hamstring stretch
- **Mobility:** Wrist/ankle/shoulder circles, arm reaches, hip circles, leg extensions

### Floor Position (36 exercises)
- **Stretches:** Child's pose, pigeon, lizard, butterfly, forward folds, wide-leg stretches, quad stretches, figure-4, happy baby, spinal twists, frog pose, half splits, runner's lunge, cobra
- **Mobility:** Hip flexor stretches, thoracic rotations, fire hydrants, hip circles, leg swings, 90-90 rotations, Cossack squats, frog pumps, glute bridges

## Language Selection

The new language selector allows you to choose your preferred language:

1. Open **Settings**
2. Find the **"Language / Språk / Langue"** section
3. Select from:
   - Auto (System Language) - default
   - Norsk (Norwegian)
   - English
   - Français (French)
4. Click **Save Changes**
5. **Restart the application** for the language change to take effect

## Managing Cues by Position

You can now filter and manage cues by position:

1. Open **Settings**
2. In the **"Manage Cues by Position"** section, select:
   - **Standing** - View/manage standing exercises
   - **Sitting** - View/manage sitting exercises
   - **Floor** - View/manage floor exercises
   - **Position Changes** - View/manage desk position adjustments
3. The list below will filter to show only cues for that position
4. Use **Add New Cue** to add position-specific exercises
5. Enable/disable cues using the checkbox in the list
6. Remove unwanted cues with **Remove Selected**

## Position Availability

Position availability is now controlled by enabling/disabling cues:
- If you don't have a standing desk, simply **disable** all standing position cues
- If you only have sitting, **disable** standing and floor cues
- The scheduler will only trigger cues that are enabled
