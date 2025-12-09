namespace ErgoHealthCue.Models;

public class CueStatistic
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CueId { get; set; }
    public string CueTitle { get; set; } = string.Empty;
    public CueType CueType { get; set; }
    public DateTime ShownAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
    public DateTime? DismissedAt { get; set; }
    public bool WasCompleted { get; set; }
}
