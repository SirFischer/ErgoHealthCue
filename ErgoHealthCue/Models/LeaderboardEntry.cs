namespace ErgoHealthCue.Models;

public class LeaderboardEntry
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int TotalXP { get; set; } = 0;
    public int CompletedCues { get; set; } = 0;
    public int DismissedCues { get; set; } = 0;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    public int TotalCues => CompletedCues + DismissedCues;
    public double CompletionRate => TotalCues > 0 ? (double)CompletedCues / TotalCues * 100 : 0;
}
