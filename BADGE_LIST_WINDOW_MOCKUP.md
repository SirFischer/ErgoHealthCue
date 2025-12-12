# Badge List Window - Visual Mockup

## Overview
The Badge List Window is accessed by clicking on the "🏆 Badges Unlocked" card in the Statistics Window. It displays all 113 available achievement badges in a card-based layout.

## Window Layout (800x650 pixels)

```
╔═══════════════════════════════════════════════════════════════════════════╗
║  🏆 Badge Achievements                                                    ║
║  5 / 113 badges unlocked                                                  ║
╠═══════════════════════════════════════════════════════════════════════════╣
║                                                                           ║
║  ┌─────────────────────────────────────────────────────────────────────┐ ║
║  │ ⚪ All Badges  ⚪ Achievement Badges  ⚪ Negative Badges  ⚪ Unlocked  │ ║
║  │    Only                                                               │ ║
║  └─────────────────────────────────────────────────────────────────────┘ ║
║                                                                           ║
║  ┌─────────────────────────────────────────────────────────────────────┐ ║
║  │ [SCROLLABLE BADGE GRID]                                             │ ║
║  │                                                                       │ ║
║  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐                │ ║
║  │  │   🏆    │  │   🏆    │  │   🏆    │  │   🔒    │                │ ║
║  │  │         │  │         │  │         │  │         │                │ ║
║  │  │  First  │  │  Baby   │  │ Getting │  │ Early   │  ...           │ ║
║  │  │  Step   │  │  Steps  │  │  Going  │  │  Bird   │                │ ║
║  │  │         │  │         │  │         │  │         │                │ ║
║  │  │Complete │  │Complete │  │Complete │  │   ???   │                │ ║
║  │  │1 cue in │  │2 cues in│  │3 cues in│  │         │                │ ║
║  │  │ a row   │  │ a row   │  │ a row   │  │         │                │ ║
║  │  └─────────┘  └─────────┘  └─────────┘  └─────────┘                │ ║
║  │  [GOLD]       [GOLD]       [GOLD]       [GREY]                      │ ║
║  │                                                                       │ ║
║  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐                │ ║
║  │  │   🔒    │  │   🔒    │  │   🔒    │  │   🔒    │                │ ║
║  │  │         │  │         │  │         │  │         │                │ ║
║  │  │ Starter │  │Beginner │  │ Novice  │  │ Learner │  ...           │ ║
║  │  │         │  │         │  │         │  │         │                │ ║
║  │  │   ???   │  │   ???   │  │   ???   │  │   ???   │                │ ║
║  │  │         │  │         │  │         │  │         │                │ ║
║  │  └─────────┘  └─────────┘  └─────────┘  └─────────┘                │ ║
║  │  [GREY]       [GREY]       [GREY]       [GREY]                      │ ║
║  │                                                                       │ ║
║  │  ... [More badges in scrollable area] ...                           │ ║
║  │                                                                       │ ║
║  │  [Negative Badges Section]                                           │ ║
║  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐                │ ║
║  │  │   💩    │  │   🔒    │  │   🔒    │  │   🔒    │                │ ║
║  │  │         │  │         │  │         │  │         │                │ ║
║  │  │ Rebel😏 │  │Procrast-│  │Dismissive│ │Too Busy│  ...           │ ║
║  │  │         │  │inator🙄 │  │   😒    │  │   🤔   │                │ ║
║  │  │Dismiss  │  │   ???   │  │   ???   │  │   ???   │                │ ║
║  │  │3 cues in│  │         │  │         │  │         │                │ ║
║  │  │ a row   │  │         │  │         │  │         │                │ ║
║  │  └─────────┘  └─────────┘  └─────────┘  └─────────┘                │ ║
║  │  [RED]        [GREY]       [GREY]       [GREY]                      │ ║
║  │                                                                       │ ║
║  └─────────────────────────────────────────────────────────────────────┘ ║
║                                                                           ║
║                                                     [    Close    ]       ║
╚═══════════════════════════════════════════════════════════════════════════╝
```

## Badge Card Details

### Unlocked Positive Badge (Gold Border)
```
┌─────────────────────┐
│        🏆           │  <- Trophy icon
│                     │
│    First Step       │  <- Badge name (gold color #F59E0B)
│                     │
│   Complete 1 cue    │  <- Requirement shown (grey #6B7280)
│     in a row        │
└─────────────────────┘
Border: 2px solid #F59E0B
Background: White (#FFFFFF)
Hover: Light gold background (#FFFBEB)
Size: 230x140 pixels
```

### Unlocked Negative Badge (Red Border)
```
┌─────────────────────┐
│        💩           │  <- Poop emoji icon
│                     │
│     Rebel 😏        │  <- Badge name (red #EF4444)
│                     │
│   Dismiss 3 cues    │  <- Requirement shown (grey #6B7280)
│     in a row        │
└─────────────────────┘
Border: 2px solid #EF4444
Background: White (#FFFFFF)
Hover: Light red background (#FEF2F2)
Size: 230x140 pixels
```

### Locked Badge (Grey)
```
┌─────────────────────┐
│        🔒           │  <- Lock icon
│                     │
│    Early Bird       │  <- Badge name (grey #9CA3AF)
│                     │
│        ???          │  <- Hidden requirement (grey #9CA3AF)
│                     │
└─────────────────────┘
Border: 1px solid #D1D5DB
Background: Light grey (#F3F4F6)
Opacity: 0.6 (60%)
No hover effect
Size: 230x140 pixels
```

## Filter Tabs

```
┌────────────────────────────────────────────────────────────┐
│  ⚪ All Badges   ⚪ Achievement Badges   ⚪ Negative Badges   │
│  ⚪ Unlocked Only                                            │
└────────────────────────────────────────────────────────────┘
```

- **All Badges** (Default) - Shows all 113 badges
- **Achievement Badges** - Shows only 100 positive badges
- **Negative Badges** - Shows only 13 negative badges
- **Unlocked Only** - Shows only earned badges

## Badge Progression Summary

### Positive Achievement Badges (100 total)
Earned by completing cues consecutively:

**Early Achievements (Badges 1-20):** 1-110 cues
- First Step, Baby Steps, Getting Going, Early Bird, Starter, Beginner, etc.

**Building Momentum (Badges 21-40):** 125-530 cues
- Go-Getter, Achiever, Striver, Worker, Grinder, etc.

**Consistent Effort (Badges 41-60):** 560-1210 cues
- Tactician, Coordinator, Director, Manager, Leader, etc.

**Advanced Dedication (Badges 61-80):** 1260-2540 cues
- Gladiator, Samurai, Ninja, Master, Expert, Professional, etc.

**Elite Tier (Badges 81-100):** 2630-10000 cues
- Sage, Wizard, Sorcerer, Oracle, Titan, Immortal, Legendary

### Negative Badges (13 total)
Earned by dismissing cues consecutively:
- Rebel 😏 (3), Procrastinator 🙄 (5), Dismissive 😒 (10)
- Too Busy? 🤔 (15), Health Denier 😤 (20), Couch Commander 🛋️ (25)
- Button Masher 🖱️ (30), Chronic Clicker 💢 (40), Ergonomic Anarchist 🏴 (50)
- Professional Ignorer 🙈 (75), Master Avoider 🏃 (100), Stubborn Sitter 🪑 (150)
- Back Pain Collector 💀 (200)

## User Flow

1. User opens Statistics Window from system tray
2. User sees "🏆 Badges Unlocked: X / 113 badges" card
3. Card shows "Click to view all badges" hint text
4. Card is visually interactive (hand cursor, hover effect)
5. User clicks on the badge card
6. Badge List Window opens as modal dialog centered on Statistics Window
7. User can filter badges by category
8. User can scroll through all badges
9. Unlocked badges show full details, locked badges show "???"
10. User clicks "Close" to return to Statistics Window

## Technical Implementation Notes

- Window uses WrapPanel for responsive badge card layout
- ItemsControl with DataTemplate for badge display
- Badge data loaded from UserProgress.CheckAndUnlockBadges definitions
- Three Style resources: BadgeCardUnlocked, BadgeCardNegative, BadgeCardLocked
- Radio buttons for filtering with Checked event handlers
- Modal dialog (ShowDialog) with Owner set to Statistics Window
