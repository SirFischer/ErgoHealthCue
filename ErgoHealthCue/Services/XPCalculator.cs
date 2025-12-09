namespace ErgoHealthCue.Services;

using ErgoHealthCue.Models;

public class XPCalculator
{
    // Base XP values for different cue types
    private const int BASE_POSITION_CHANGE_XP = 10;
    private const int BASE_STANDING_XP = 25;
    private const int BASE_SITTING_XP = 15;
    private const int BASE_FLOOR_XP = 35;
    
    // Penalty for dismissing or timing out (50% of potential gain)
    private const double PENALTY_MULTIPLIER = 0.5;
    
    public int CalculateXPGain(CueType cueType, int intervalMinutes, bool completed)
    {
        int baseXP = GetBaseXP(cueType);
        
        // Moderate based on interval - longer intervals = slightly less XP to prevent gaming
        // Formula: baseXP * (1 - (intervalMinutes - 30) / 300)
        // At 30 min: 100% XP, at 60 min: 90% XP, at 120 min: 70% XP
        double intervalModifier = 1.0 - Math.Max(0, (intervalMinutes - 30)) / 300.0;
        intervalModifier = Math.Max(0.7, Math.Min(1.0, intervalModifier));
        
        int modifiedXP = (int)(baseXP * intervalModifier);
        
        // Apply penalty if dismissed or timed out
        if (!completed)
        {
            modifiedXP = -(int)(modifiedXP * PENALTY_MULTIPLIER);
        }
        
        return modifiedXP;
    }
    
    private int GetBaseXP(CueType cueType)
    {
        return cueType switch
        {
            CueType.DeskStanding => BASE_POSITION_CHANGE_XP,
            CueType.DeskSitting => BASE_POSITION_CHANGE_XP,
            CueType.DeskFloor => BASE_POSITION_CHANGE_XP,
            CueType.StandingStretch => BASE_STANDING_XP,
            CueType.StandingMobilityDrill => BASE_STANDING_XP,
            CueType.SittingStretch => BASE_SITTING_XP,
            CueType.SittingMobilityDrill => BASE_SITTING_XP,
            CueType.FloorStretch => BASE_FLOOR_XP,
            CueType.FloorMobilityDrill => BASE_FLOOR_XP,
            _ => 10
        };
    }
    
    public string GetXPDescription(CueType cueType)
    {
        return cueType switch
        {
            CueType.FloorStretch or CueType.FloorMobilityDrill => "High XP (Floor exercise)",
            CueType.StandingStretch or CueType.StandingMobilityDrill => "Medium XP (Standing exercise)",
            CueType.SittingStretch or CueType.SittingMobilityDrill => "Low XP (Sitting exercise)",
            _ => "Minimal XP (Position change)"
        };
    }
}
