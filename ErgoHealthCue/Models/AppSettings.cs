namespace ErgoHealthCue.Models;

public class AppSettings
{
    // Exercise intervals
    public int MinExerciseIntervalMinutes { get; set; } = 30;
    public int MaxExerciseIntervalMinutes { get; set; } = 45;
    public bool UseRandomExerciseIntervals { get; set; } = true;
    
    // Position change intervals
    public int MinPositionIntervalMinutes { get; set; } = 60;
    public int MaxPositionIntervalMinutes { get; set; } = 120;
    public bool UseRandomPositionIntervals { get; set; } = true;
    
    // Position availability
    public bool StandingPositionAvailable { get; set; } = true;
    public bool SittingPositionAvailable { get; set; } = true;
    public bool FloorPositionAvailable { get; set; } = true;
    
    public bool StartOnWindowsStartup { get; set; } = false;
    public DeskPosition CurrentPosition { get; set; } = DeskPosition.Sitting;
    public string Language { get; set; } = "auto"; // "auto", "no", "en", "fr"
    public List<Cue> Cues { get; set; } = new();
    public UserProgress Progress { get; set; } = new();
}
