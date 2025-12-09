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

    private AppSettings CreateDefaultSettings()
    {
        return new AppSettings
        {
            MinIntervalMinutes = 30,
            MaxIntervalMinutes = 60,
            UseRandomIntervals = true,
            StartOnWindowsStartup = false,
            CurrentPosition = DeskPosition.Sitting,
            Cues = new List<Cue>
            {
                // Position changes
                new() { Type = CueType.DeskStanding, Title = "Stand Up", Description = "Raise your desk to standing position" },
                new() { Type = CueType.DeskSitting, Title = "Sit Down", Description = "Lower your desk to sitting position" },
                new() { Type = CueType.DeskFloor, Title = "Floor Position", Description = "Lower your desk all the way down" },
                
                // Standing position exercises
                new() { Type = CueType.StandingStretch, Title = "Standing Neck Stretch", Description = "While standing, gently tilt your head to each side, holding for 10 seconds" },
                new() { Type = CueType.StandingStretch, Title = "Standing Shoulder Rolls", Description = "Roll your shoulders backwards 10 times while standing" },
                new() { Type = CueType.StandingStretch, Title = "Standing Calf Stretch", Description = "Step forward and gently stretch your calf muscles" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Hip Circles", Description = "Make large circles with your hips, 10 in each direction" },
                new() { Type = CueType.StandingMobilityDrill, Title = "Ankle Mobility", Description = "Lift one foot and rotate your ankle 10 times each direction" },
                
                // Sitting position exercises
                new() { Type = CueType.SittingStretch, Title = "Seated Neck Stretch", Description = "Gently tilt your head to each side while seated, holding for 10 seconds" },
                new() { Type = CueType.SittingStretch, Title = "Seated Shoulder Shrugs", Description = "Shrug your shoulders up to your ears and release, 10 times" },
                new() { Type = CueType.SittingStretch, Title = "Seated Twist", Description = "Twist your torso to each side, holding for 10 seconds" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Wrist Circles", Description = "Rotate your wrists 10 times in each direction" },
                new() { Type = CueType.SittingMobilityDrill, Title = "Seated Ankle Pumps", Description = "Point and flex your feet 15 times" },
                
                // Floor position exercises
                new() { Type = CueType.FloorStretch, Title = "Child's Pose", Description = "Kneel down and reach your arms forward, stretching your back" },
                new() { Type = CueType.FloorStretch, Title = "Cat-Cow Stretch", Description = "On all fours, alternate between arching and rounding your back" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Hip Flexor Stretch", Description = "Kneel on one knee and lean forward to stretch your hip flexors" },
                new() { Type = CueType.FloorMobilityDrill, Title = "Thoracic Rotation", Description = "On all fours, rotate your upper body side to side" }
            }
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
