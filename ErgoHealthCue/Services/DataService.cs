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
            Cues = new List<Cue>
            {
                new() { Type = CueType.DeskStanding, Title = "Stand Up", Description = "Raise your desk to standing position" },
                new() { Type = CueType.DeskSitting, Title = "Sit Down", Description = "Lower your desk to sitting position" },
                new() { Type = CueType.DeskFloor, Title = "Floor Position", Description = "Lower your desk all the way down" },
                new() { Type = CueType.Stretch, Title = "Neck Stretch", Description = "Gently tilt your head to each side, holding for 10 seconds" },
                new() { Type = CueType.Stretch, Title = "Shoulder Rolls", Description = "Roll your shoulders backwards 10 times" },
                new() { Type = CueType.MobilityDrill, Title = "Hip Circles", Description = "Stand and make large circles with your hips, 10 each direction" },
                new() { Type = CueType.MobilityDrill, Title = "Ankle Mobility", Description = "Rotate each ankle 10 times in each direction" }
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
