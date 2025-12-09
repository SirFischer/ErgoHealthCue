namespace ErgoHealthCue.Models;

public class AppSettings
{
    public int MinIntervalMinutes { get; set; } = 30;
    public int MaxIntervalMinutes { get; set; } = 60;
    public bool UseRandomIntervals { get; set; } = true;
    public bool StartOnWindowsStartup { get; set; } = false;
    public DeskPosition CurrentPosition { get; set; } = DeskPosition.Sitting;
    public List<Cue> Cues { get; set; } = new();
}
