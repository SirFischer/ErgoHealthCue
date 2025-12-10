namespace ErgoHealthCue.Models;

public class Cue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public CueType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
