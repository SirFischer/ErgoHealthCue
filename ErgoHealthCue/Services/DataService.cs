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
            
            // Standing position exercises - Dynamic movements for circulation and mobility (15 exercises)
            new() { Type = CueType.StandingStretch, Title = "Standing Chest Opener", Description = "Clasp hands behind back, straighten arms, lift chest up and back, hold 20 seconds" },
            new() { Type = CueType.StandingStretch, Title = "Standing Hip Flexor Stretch", Description = "Step back into lunge, sink hips forward, feel stretch in front of back leg, 20 seconds each" },
            new() { Type = CueType.StandingStretch, Title = "Standing Quad Stretch", Description = "Hold foot behind you, pull heel to glutes, keep knees together, 20 seconds each leg" },
            new() { Type = CueType.StandingStretch, Title = "Standing Calf Stretch", Description = "Step forward, keep back leg straight, press back heel down, 20 seconds each leg" },
            new() { Type = CueType.StandingStretch, Title = "Standing Side Bend", Description = "Reach one arm overhead, bend to opposite side, stretch entire side body, 15 seconds each" },
            new() { Type = CueType.StandingStretch, Title = "Standing Hamstring Stretch", Description = "Place one heel on desk edge, keep leg straight, reach toward toes, 20 seconds each leg" },
            new() { Type = CueType.StandingStretch, Title = "Standing Shoulder Stretch", Description = "Pull one arm across chest, use other arm to deepen stretch, 15 seconds each side" },
            new() { Type = CueType.StandingStretch, Title = "Standing Triceps Stretch", Description = "Reach one arm overhead, bend elbow, pull gently with other hand behind head, 15 seconds each" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Hip Circles", Description = "Hands on hips, make large circles, loosen hip joints, 10 each direction" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Leg Swings", Description = "Hold desk for balance, swing one leg forward/back 10 times, then side-to-side 10 times, each leg" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Torso Twists", Description = "Twist torso left and right with control, arms loose, 20 total reps" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Shoulder Rolls & Arm Circles", Description = "Roll shoulders backward 10 times, then make arm circles forward and back, 10 each" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing March", Description = "March in place, lift knees high, pump arms, increase blood flow, 30 seconds" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Ankle Circles", Description = "Lift one foot off ground, rotate ankle 10 times clockwise, then 10 counterclockwise, each foot" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Wrist Rolls", Description = "Extend arms forward, roll wrists in circles 10 times each direction, release tension" },
            
            // Sitting position exercises - Upper body, neck, wrists for desk workers (15 exercises)
            new() { Type = CueType.SittingStretch, Title = "Seated Neck Stretch", Description = "Tilt head to side, gently pull with hand for deeper stretch, 15 seconds each side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Upper Trap Stretch", Description = "Drop one ear to shoulder, opposite hand behind back, 15 seconds each side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Chest Stretch", Description = "Clasp hands behind chair back, lift chest, squeeze shoulder blades, hold 20 seconds" },
            new() { Type = CueType.SittingStretch, Title = "Seated Torso Twist", Description = "Twist to one side, use chair for leverage, keep hips square, 15 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Figure-4 Hip Stretch", Description = "Cross ankle over opposite knee, sit tall, lean forward slightly, 20 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Hamstring Stretch", Description = "Extend one leg straight, flex foot, hinge at hips to reach forward, 20 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Side Stretch", Description = "Reach one arm overhead and lean to opposite side, feel stretch along ribcage, 15 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Spinal Extension", Description = "Hands on lower back, gently arch backward, open chest, hold 15 seconds" },
            new() { Type = CueType.SittingStretch, Title = "Seated Wrist Flexor Stretch", Description = "Extend arm forward, pull fingers back with other hand, 15 seconds each side" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Wrist Circles & Flexion", Description = "Circle wrists 10 times each direction, then flex and extend wrists 15 times" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Shoulder Circles", Description = "Circle shoulders backward 10 times, then forward 10 times, focus on full range" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Scapular Squeezes", Description = "Squeeze shoulder blades together, hold 3 seconds, release, 12 reps" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Chin Tucks", Description = "Pull chin straight back (double chin), hold 5 seconds, strengthen neck, 10 reps" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Cat-Cow", Description = "Round spine forward (cat), then arch back (cow), hands on knees, 10 slow cycles" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Ankle Pumps", Description = "Lift feet slightly, point toes down then pull up, improve circulation, 20 reps" },
            
            // Floor position exercises - Deep stretches, hip mobility, splits prep (15 exercises)
            new() { Type = CueType.FloorStretch, Title = "Child's Pose", Description = "Kneel, sit back on heels, reach arms forward, relax spine and shoulders, 30 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Cat-Cow Stretch", Description = "On all fours, alternate arching back (cow) and rounding spine (cat), 10 slow cycles" },
            new() { Type = CueType.FloorStretch, Title = "Pigeon Pose", Description = "One leg bent in front, other extended back, sink into hips, deep hip opener, 30 seconds each" },
            new() { Type = CueType.FloorStretch, Title = "90-90 Hip Stretch", Description = "Both legs bent at 90 degrees, lean forward over front leg, 30 seconds each position" },
            new() { Type = CueType.FloorStretch, Title = "Seated Forward Fold", Description = "Legs extended, reach for toes, keep back straight, hamstring stretch, 30 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Butterfly Stretch", Description = "Soles of feet together, press knees down gently, lean forward slightly, 30 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Lizard Pose", Description = "Low lunge with forearms on ground, deep hip flexor and groin stretch, 25 seconds each" },
            new() { Type = CueType.FloorStretch, Title = "Supine Spinal Twist", Description = "Lie on back, drop knees to one side, arms out, gentle spine rotation, 20 seconds each" },
            new() { Type = CueType.FloorStretch, Title = "Cobra Stretch", Description = "Lie face down, push chest up with arms, arch back gently, hold 20 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Kneeling Hip Flexor Stretch", Description = "Kneel on one knee, other foot forward, push hips forward, 25 seconds each side" },
            new() { Type = CueType.FloorStretch, Title = "Frog Stretch", Description = "On all fours, knees wide apart, sink hips back and down, deep groin stretch, 30 seconds" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Hip Circles (Quadruped)", Description = "On all fours, lift one knee out and circle it 10 times each direction, each leg" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Glute Bridges", Description = "Lie on back, feet flat, lift hips high, squeeze glutes at top, 15 reps" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Quadruped Thoracic Rotations", Description = "On all fours, one hand behind head, rotate torso open, 10 times each side" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Kneeling Side Bend Flow", Description = "Kneel upright, reach one arm overhead and bend to side, alternate sides smoothly, 10 each side" }
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
    
    /// <summary>
    /// Generates a default username from a user ID.
    /// Safely handles short IDs.
    /// </summary>
    public static string GenerateDefaultUsername(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return "User";
        }
        
        int length = Math.Min(8, userId.Length);
        return $"User{userId.Substring(0, length)}";
    }
}
