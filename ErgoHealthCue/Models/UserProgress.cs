namespace ErgoHealthCue.Models;

public class UserProgress
{
    public int Level { get; set; } = 1;
    public int CurrentXP { get; set; } = 0;
    public int TotalXPEarned { get; set; } = 0;
    
    // Streak tracking
    public int CurrentStreak { get; set; } = 0;
    public int BestStreak { get; set; } = 0;
    public List<string> UnlockedBadges { get; set; } = new();
    
    public int XPForNextLevel()
    {
        // Progressive XP requirement adjusted for ~3 years to reach level 100
        // With ~10 cues/day averaging 20 XP each = ~200 XP/day = ~73,000 XP/year
        // Total for 3 years: ~219,000 XP
        // Level 1->2: 500 XP, Level 2->3: 1000 XP, Level 3->4: 1500 XP, etc.
        return Level * 500;
    }
    
    public void AddXP(int xp)
    {
        CurrentXP += xp;
        TotalXPEarned += xp;
        
        // Check for level up
        while (CurrentXP >= XPForNextLevel() && Level < 100)
        {
            CurrentXP -= XPForNextLevel();
            Level++;
        }
        
        // Cap at level 100
        if (Level >= 100)
        {
            Level = 100;
            CurrentXP = 0;
        }
    }
    
    public void RemoveXP(int xp)
    {
        CurrentXP -= xp;
        if (CurrentXP < 0)
        {
            CurrentXP = 0;
        }
    }
    
    public int GetProgressPercentage()
    {
        if (Level >= 100) return 100;
        return (int)((double)CurrentXP / XPForNextLevel() * 100);
    }
    
    public void IncrementStreak()
    {
        CurrentStreak++;
        if (CurrentStreak > BestStreak)
        {
            BestStreak = CurrentStreak;
        }
    }
    
    public void BreakStreak()
    {
        CurrentStreak = 0;
    }
    
    public List<string> CheckAndUnlockBadges()
    {
        var newBadges = new List<string>();
        
        // Badge definitions - adjusted to take several years to complete
        // At ~10 cues/day, achievements span from 1 day to ~3 years
        var badgeDefinitions = new Dictionary<string, int>
        {
            { "First Steps", 10 },           // 10 in a row (~1 day)
            { "Getting Started", 50 },       // 50 in a row (~5 days)
            { "Building Habits", 100 },      // 100 in a row (~2 weeks)
            { "Dedication", 250 },           // 250 in a row (~1 month)
            { "Committed", 500 },            // 500 in a row (~2 months)
            { "Champion", 1000 },            // 1000 in a row (~4 months)
            { "Master", 2500 },              // 2500 in a row (~10 months)
            { "Grandmaster", 5000 },         // 5000 in a row (~1.5 years)
            { "Legend", 10000 }              // 10000 in a row (~3 years)
        };
        
        foreach (var badge in badgeDefinitions)
        {
            if (CurrentStreak >= badge.Value && !UnlockedBadges.Contains(badge.Key))
            {
                UnlockedBadges.Add(badge.Key);
                newBadges.Add(badge.Key);
            }
        }
        
        return newBadges;
    }
}
