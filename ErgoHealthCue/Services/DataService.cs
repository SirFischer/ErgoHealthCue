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
            
            // Standing position exercises - Professional stretches suitable for office (20 exercises)
            new() { Type = CueType.StandingStretch, Title = "Standing Chest Opener", Description = "Clasp hands behind back, straighten arms, lift chest up and back, hold 20 seconds" },
            new() { Type = CueType.StandingStretch, Title = "Standing Calf Stretch", Description = "Step one foot back, keep back leg straight, press heel down gently, 20 seconds each leg" },
            new() { Type = CueType.StandingStretch, Title = "Standing Side Bend", Description = "Reach one arm overhead, bend to opposite side, stretch entire side body, 15 seconds each" },
            new() { Type = CueType.StandingStretch, Title = "Standing Shoulder Stretch", Description = "Pull one arm across chest, use other arm to deepen stretch, 15 seconds each side" },
            new() { Type = CueType.StandingStretch, Title = "Standing Triceps Stretch", Description = "Reach one arm overhead, bend elbow, pull gently with other hand behind head, 15 seconds each" },
            new() { Type = CueType.StandingStretch, Title = "Standing Neck Side Stretch", Description = "Gently tilt head to one side, use hand to apply light pressure, hold 15 seconds each side" },
            new() { Type = CueType.StandingStretch, Title = "Standing Forearm Stretch", Description = "Extend arm forward, gently pull fingers back with other hand, then pull fingers down, 15 seconds each" },
            new() { Type = CueType.StandingStretch, Title = "Standing Back Extension", Description = "Place hands on lower back, gently lean backward, look up slightly, hold 15 seconds" },
            new() { Type = CueType.StandingStretch, Title = "Standing Lat Stretch", Description = "Reach both arms overhead, grab one wrist and pull gently to side, 15 seconds each side" },
            new() { Type = CueType.StandingStretch, Title = "Standing Upper Back Stretch", Description = "Clasp hands in front, round upper back, push hands away from body, 20 seconds" },
            new() { Type = CueType.StandingStretch, Title = "Standing Neck Rotation", Description = "Slowly turn head to look over shoulder, hold 15 seconds each side, release tension" },
            new() { Type = CueType.StandingStretch, Title = "Standing Bicep Stretch", Description = "Extend arm to side at shoulder height, gently press against wall or doorframe, 15 seconds each" },
            new() { Type = CueType.StandingStretch, Title = "Standing Prayer Stretch", Description = "Press palms together in front of chest, push hands down while keeping palms together, 15 seconds" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Torso Twists", Description = "Slowly twist torso left and right with control, arms relaxed, 20 total reps" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Shoulder Rolls", Description = "Slowly roll shoulders backward 10 times, then forward 10 times, focus on full range" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Ankle Circles", Description = "Lift one foot slightly off ground, slowly rotate ankle 10 times each direction, each foot" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Wrist Rolls", Description = "Extend arms forward, slowly roll wrists in circles 10 times each direction, release tension" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Arm Raises", Description = "Slowly raise both arms to sides and overhead, lower down, repeat 10 times, gentle movement" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Shoulder Blade Squeezes", Description = "Pull shoulders back, squeeze shoulder blades together, hold 5 seconds, release, 10 reps" },
            new() { Type = CueType.StandingMobilityDrill, Title = "Standing Neck Tilts", Description = "Gently tilt head forward, hold 10 seconds, then tilt to each side, 10 seconds each, slow and controlled" },
            
            // Sitting position exercises - Upper body, neck, wrists for desk workers (20 exercises)
            new() { Type = CueType.SittingStretch, Title = "Seated Neck Stretch", Description = "Tilt head to side, gently pull with hand for deeper stretch, 15 seconds each side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Upper Trap Stretch", Description = "Drop one ear to shoulder, opposite hand behind back, 15 seconds each side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Chest Stretch", Description = "Clasp hands behind chair back, lift chest, squeeze shoulder blades, hold 20 seconds" },
            new() { Type = CueType.SittingStretch, Title = "Seated Torso Twist", Description = "Twist to one side, use chair for leverage, keep hips square, 15 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Figure-4 Hip Stretch", Description = "Cross ankle over opposite knee, sit tall, lean forward slightly, 20 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Hamstring Stretch", Description = "Extend one leg straight, flex foot, hinge at hips to reach forward, 20 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Side Stretch", Description = "Reach one arm overhead and lean to opposite side, feel stretch along ribcage, 15 seconds each" },
            new() { Type = CueType.SittingStretch, Title = "Seated Spinal Extension", Description = "Hands on lower back, gently arch backward, open chest, hold 15 seconds" },
            new() { Type = CueType.SittingStretch, Title = "Seated Wrist Flexor Stretch", Description = "Extend arm forward, pull fingers back with other hand, 15 seconds each side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Neck Rotation", Description = "Slowly turn head to look over one shoulder, hold 15 seconds, repeat other side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Forward Fold", Description = "Sit at edge of chair, hinge forward from hips, let arms and head hang, relax spine, 20 seconds" },
            new() { Type = CueType.SittingStretch, Title = "Seated Wrist Extensor Stretch", Description = "Extend arm forward, gently pull fingers down and in with other hand, 15 seconds each side" },
            new() { Type = CueType.SittingStretch, Title = "Seated Shoulder Blade Squeeze", Description = "Pull shoulders back, squeeze shoulder blades together, hold 10 seconds, release, repeat 8 times" },
            new() { Type = CueType.SittingStretch, Title = "Seated Hip Opener", Description = "Sit in chair, place ankle on opposite knee, gently press down on raised knee, 15 seconds each side" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Wrist Circles", Description = "Slowly circle wrists 10 times each direction, then flex and extend wrists gently 15 times" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Shoulder Circles", Description = "Slowly circle shoulders backward 10 times, then forward 10 times, focus on full range" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Scapular Squeezes", Description = "Squeeze shoulder blades together, hold 3 seconds, release slowly, 12 reps" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Chin Tucks", Description = "Gently pull chin straight back (double chin), hold 5 seconds, strengthen neck, 10 reps" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Spinal Waves", Description = "Slowly round spine forward, then arch back gently, hands on knees for support, 10 slow cycles" },
            new() { Type = CueType.SittingMobilityDrill, Title = "Seated Ankle Pumps", Description = "Lift feet slightly, slowly point toes down then pull up, improve circulation, 20 reps" },
            
            // Floor position exercises - Professional seated stretches suitable for office (20 exercises)
            new() { Type = CueType.FloorStretch, Title = "Seated Cross-Legged Stretch", Description = "Sit cross-legged, place hands on knees, sit tall and breathe deeply, 30 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Seated Forward Reach", Description = "Sit with legs extended forward, reach arms forward toward toes, keep back straight, 25 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Seated Side Stretch", Description = "Sit cross-legged, reach one arm overhead and lean to side, 20 seconds each side" },
            new() { Type = CueType.FloorStretch, Title = "Seated Spinal Twist", Description = "Sit with legs extended, place one hand behind you, twist torso gently, 20 seconds each side" },
            new() { Type = CueType.FloorStretch, Title = "Seated Neck Stretch", Description = "Sit cross-legged, tilt head to one side, use hand for gentle pressure, 15 seconds each side" },
            new() { Type = CueType.FloorStretch, Title = "Seated Shoulder Rolls", Description = "Sit cross-legged, slowly roll shoulders backward 10 times, then forward 10 times" },
            new() { Type = CueType.FloorStretch, Title = "Seated Chest Opener", Description = "Sit cross-legged, clasp hands behind back, straighten arms, lift chest, 20 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Seated Upper Back Stretch", Description = "Sit cross-legged, reach arms forward, clasp hands, round upper back, 20 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Seated Arm Overhead Stretch", Description = "Sit cross-legged, reach both arms overhead, grab one wrist and pull to side, 15 seconds each" },
            new() { Type = CueType.FloorStretch, Title = "Seated Wrist Stretch", Description = "Sit cross-legged, extend one arm forward, pull fingers back gently, 15 seconds each side" },
            new() { Type = CueType.FloorStretch, Title = "Seated Triceps Stretch", Description = "Sit cross-legged, reach one arm overhead, bend elbow, pull with other hand, 15 seconds each" },
            new() { Type = CueType.FloorStretch, Title = "Seated Hip Stretch", Description = "Sit with one leg bent in front, other leg bent and resting to side, lean forward slightly over front leg, 20 seconds each" },
            new() { Type = CueType.FloorStretch, Title = "Seated Butterfly Position", Description = "Sit with soles of feet together, hold feet, sit tall with straight spine, 25 seconds" },
            new() { Type = CueType.FloorStretch, Title = "Seated Ankle Circles", Description = "Sit with legs extended, rotate ankles slowly 10 times each direction" },
            new() { Type = CueType.FloorStretch, Title = "Seated Back Extension", Description = "Sit cross-legged, place hands on lower back, gently arch back, look up slightly, 15 seconds" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Seated Torso Twists", Description = "Sit cross-legged, place hands on opposite knees, slowly twist side to side, 20 total reps" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Seated Arm Circles", Description = "Sit cross-legged, extend arms to sides, make small circles, 10 forward then 10 backward" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Seated Shoulder Blade Squeezes", Description = "Sit cross-legged, squeeze shoulder blades together, hold 5 seconds, release, 10 reps" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Seated Neck Rotations", Description = "Sit cross-legged, slowly turn head to look over each shoulder, 10 times each side" },
            new() { Type = CueType.FloorMobilityDrill, Title = "Seated Breathing Exercise", Description = "Sit cross-legged with hands on knees, take slow deep breaths, focus on posture, 30 seconds" }
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
