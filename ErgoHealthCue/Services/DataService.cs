using System.IO;
using System.Text.Json;
using ErgoHealthCue.Models;

namespace ErgoHealthCue.Services;

public class DataService
{
    private readonly string _settingsPath;
    private readonly string _statisticsPath;
    private readonly string _appDataFolder;
    private readonly object _statisticsLock = new();

    public DataService()
    {
        _appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ErgoHealthCue"
        );
        
        Directory.CreateDirectory(_appDataFolder);
        
        _settingsPath = Path.Combine(_appDataFolder, "settings.json");
        _statisticsPath = Path.Combine(_appDataFolder, "statistics.json");
    }

    public AppSettings LoadSettings()
    {
        if (!File.Exists(_settingsPath))
        {
            var defaultSettings = CreateDefaultSettings();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        try
        {
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? CreateDefaultSettings();
        }
        catch
        {
            return CreateDefaultSettings();
        }
    }

    public void SaveSettings(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(_settingsPath, json);
    }

    public List<Cue> GetDefaultCues()
    {
        return CreateDefaultCues();
    }

    private AppSettings CreateDefaultSettings()
    {
        return new AppSettings
        {
            MinExerciseIntervalMinutes = 30,
            MaxExerciseIntervalMinutes = 45,
            UseRandomExerciseIntervals = true,
            MinPositionIntervalMinutes = 60,
            MaxPositionIntervalMinutes = 120,
            UseRandomPositionIntervals = true,
            StandingPositionAvailable = true,
            SittingPositionAvailable = true,
            FloorPositionAvailable = true,
            StartOnWindowsStartup = false,
            CurrentPosition = DeskPosition.Sitting,
            Language = "auto",
            Cues = CreateDefaultCues()
        };
    }

    private List<Cue> CreateDefaultCues()
    {
        return new List<Cue>
        {
            // Position changes
            new() { Type = CueType.DeskStanding, Title = "Stand Up", Description = "Raise your desk to standing position" },
            new() { Type = CueType.DeskSitting, Title = "Sit Down", Description = "Lower your desk to sitting position" },
            new() { Type = CueType.DeskFloor, Title = "Floor Position", Description = "Lower your desk all the way down" },
            
            // Standing position exercises - More dynamic and unrestricted
            new() { Type = CueType.StandingStretch, Title = "Standing Neck Stretch", Description = "Gently tilt your head to each side, holding for 10 seconds" },
            new() { Type = CueType.StandingStretch, Title = "Standing Shoulder Rolls", Description = "Roll your shoulders backwards 10 times" },
                new() { Type = CueType.StandingStretch, Title = "Standing Calf Stretch", Description = "Step forward and gently stretch your calf muscles, hold 15 seconds each leg" },
                new() { Type = CueType.StandingStretch, Title = "Standing Quad Stretch", Description = "Hold your foot behind you, pulling heel to glutes, hold 15 seconds each leg" },
                new() { Type = CueType.StandingStretch, Title = "Standing Side Bend", Description = "Reach one arm overhead and bend to the side, hold 10 seconds each side" },
                new() { Type = CueType.StandingStretch, Title = "Standing Forward Fold", Description = "Hinge at hips, let arms hang down, gentle stretch for hamstrings and back" },
                new() { Type = CueType.StandingStretch, Title = "Standing Chest Opener", Description = "Clasp hands behind back, straighten arms, lift chest, hold 15 seconds" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Hip Circles", Description = "Make large circles with your hips, 10 in each direction" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Ankle Mobility", Description = "Lift one foot and rotate your ankle 10 times each direction" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Standing Leg Swings", Description = "Swing one leg forward and back 10 times, then side to side 10 times each leg" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Standing Torso Twists", Description = "Twist your torso left and right dynamically, 20 total reps" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Standing March", Description = "March in place, lifting knees high, 30 seconds" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Standing Arm Circles", Description = "Make large circles with both arms, 10 forward, 10 backward" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Standing Hip Flexor Kicks", Description = "Alternate kicking legs forward, 20 total kicks" },
                
                // Sitting position exercises - Hip openers, neck, upper body
                new() { Type = CueType.SittingStretch, Title = "Seated Neck Stretch", Description = "Gently tilt your head to each side, holding for 10 seconds" },
                new() { Type = CueType.SittingStretch, Title = "Seated Neck Rotations", Description = "Slowly turn head left and right, hold 5 seconds each side, 5 reps" },
                new() { Type = CueType.SittingStretch, Title = "Seated Shoulder Shrugs", Description = "Shrug shoulders up to ears and release, 10 times" },
                new() { Type = CueType.SittingStretch, Title = "Seated Upper Trap Stretch", Description = "Tilt head to side, use hand to gently increase stretch, 15 seconds each" },
                new() { Type = CueType.SittingStretch, Title = "Seated Chest Stretch", Description = "Clasp hands behind chair, straighten arms, lift chest, hold 20 seconds" },
                new() { Type = CueType.SittingStretch, Title = "Seated Torso Twist", Description = "Twist torso to each side using chair for support, hold 15 seconds" },
                new() { Type = CueType.SittingStretch, Title = "Seated Forward Fold", Description = "Hinge at hips, fold forward over thighs, relax neck, hold 20 seconds" },
                new() { Type = CueType.SittingStretch, Title = "Seated Side Bend", Description = "Reach one arm overhead, bend to the opposite side, 15 seconds each" },
                new() { Type = CueType.SittingStretch, Title = "Seated Figure-4 Hip Stretch", Description = "Cross ankle over opposite knee, lean forward for hip stretch, 20 seconds each" },
                new() { Type = CueType.SittingStretch, Title = "Seated Hip Flexor Stretch", Description = "Scoot to edge of chair, extend one leg back, feel hip flexor stretch, 20 seconds" },
                new() { Type = CueType.SittingStretch, Title = "Seated Hamstring Stretch", Description = "Extend one leg, flex foot, lean forward, 20 seconds each leg" },
                new() { Type = CueType.SittingStretch, Title = "Seated Glute Stretch", Description = "Cross one ankle over knee, press down gently on knee, 20 seconds each" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Wrist Circles", Description = "Rotate wrists 10 times in each direction" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Ankle Pumps", Description = "Point and flex feet 15 times" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Ankle Circles", Description = "Rotate ankles 10 times each direction, both feet" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Shoulder Circles", Description = "Circle shoulders forward 10 times, then backward 10 times" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Arm Reaches", Description = "Reach arms overhead, then out to sides, 10 reps" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Hip Circles", Description = "Make circles with your hips while seated, 10 each direction" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Leg Extensions", Description = "Extend one leg at a time, hold briefly, 10 reps each leg" },
                
                // Floor position exercises - Hip openers, hamstrings, quads, splits training
                new() { Type = CueType.FloorStretch, Title = "Child's Pose", Description = "Kneel, sit back on heels, reach arms forward, relax for 30 seconds" },
                new() { Type = CueType.FloorStretch, Title = "Cat-Cow Stretch", Description = "On all fours, alternate arching and rounding back, 10 cycles" },
                new() { Type = CueType.FloorStretch, Title = "Pigeon Pose", Description = "One leg bent in front, other extended back, hold for hip opening, 30 seconds each" },
                new() { Type = CueType.FloorStretch, Title = "Lizard Pose", Description = "Lunge position with forearms on ground, deep hip flexor stretch, 30 seconds each" },
                new() { Type = CueType.FloorStretch, Title = "Butterfly Stretch", Description = "Sit with soles of feet together, press knees down gently, hold 30 seconds" },
                new() { Type = CueType.FloorStretch, Title = "Seated Forward Fold", Description = "Legs extended, reach for toes, hamstring stretch, hold 30 seconds" },
                new() { Type = CueType.FloorStretch, Title = "Wide-Leg Forward Fold", Description = "Legs wide apart, fold forward between them, 30 seconds" },
                new() { Type = CueType.FloorStretch, Title = "Single Leg Hamstring Stretch", Description = "One leg extended, fold forward over it, 30 seconds each leg" },
                new() { Type = CueType.FloorStretch, Title = "Quad Stretch (Lying)", Description = "Lie on side, pull top foot to glutes, 30 seconds each leg" },
                new() { Type = CueType.FloorStretch, Title = "Reclined Figure-4 Stretch", Description = "Lie on back, cross ankle over knee, pull knee to chest, 30 seconds each" },
                new() { Type = CueType.FloorStretch, Title = "Happy Baby Pose", Description = "Lie on back, hold outside of feet, knees bent, rock gently, 30 seconds" },
                new() { Type = CueType.FloorStretch, Title = "Supine Spinal Twist", Description = "Lie on back, drop knees to side, arms out, 20 seconds each side" },
                new() { Type = CueType.FloorStretch, Title = "Frog Pose", Description = "On knees, spread knees wide, forearms down, hips back, deep hip opener, 30 seconds" },
                new() { Type = CueType.FloorStretch, Title = "Half Split Stretch", Description = "One leg extended, other folded, lean forward for hamstring stretch, 30 seconds each" },
                new() { Type = CueType.FloorStretch, Title = "Runner's Lunge", Description = "Lunge position, back knee down, hands on ground, 20 seconds each side" },
                new() { Type = CueType.FloorStretch, Title = "Cobra Stretch", Description = "Lie on stomach, push chest up with arms, gentle back extension, 20 seconds" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Hip Flexor Stretch (Kneeling)", Description = "Kneel on one knee, lunge forward, feel front of hip, 25 seconds each" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Thoracic Rotation", Description = "On all fours, rotate upper body side to side, 10 each side" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Fire Hydrants", Description = "On all fours, lift one knee out to side, 10 reps each leg" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Hip Circles (Quadruped)", Description = "On all fours, make circles with one knee, 10 each direction, each leg" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Leg Swings (Floor)", Description = "On all fours, swing one leg forward to chest then back, 10 reps each" },
                new() { Type = CueType.FloorMobilityDrill, Title = "90-90 Hip Rotations", Description = "Sit with legs in 90-90 position, rotate to switch sides, 10 transitions" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Cossack Squats", Description = "Wide stance, shift weight side to side, straighten one leg, 10 each side" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Frog Pumps", Description = "Lie on back, soles of feet together, lift hips up and down, 15 reps" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Glute Bridges", Description = "Lie on back, feet flat, lift hips up, squeeze glutes, 15 reps" }
        };
    }

    public List<CueStatistic> LoadStatistics()
    {
        if (!File.Exists(_statisticsPath))
        {
            return new List<CueStatistic>();
        }

        try
        {
            var json = File.ReadAllText(_statisticsPath);
            return JsonSerializer.Deserialize<List<CueStatistic>>(json) ?? new List<CueStatistic>();
        }
        catch
        {
            return new List<CueStatistic>();
        }
    }

    public void SaveStatistics(List<CueStatistic> statistics)
    {
        var json = JsonSerializer.Serialize(statistics, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(_statisticsPath, json);
    }

    public void AddStatistic(CueStatistic statistic)
    {
        lock (_statisticsLock)
        {
            var statistics = LoadStatistics();
            statistics.Add(statistic);
            SaveStatistics(statistics);
        }
    }
}
