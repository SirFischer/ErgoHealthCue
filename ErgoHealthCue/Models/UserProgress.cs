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
        // Progressive XP requirement: Level * 100 XP
        // Level 1->2: 100 XP, Level 2->3: 200 XP, etc.
        return Level * 100;
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
        
        // Badge definitions - achievable during work hours
        var badgeDefinitions = new Dictionary<string, int>
        {
            { "First Steps", 3 },        // 3 in a row (very easy - first day)
            { "Getting Started", 5 },     // 5 in a row (easy - first day)
            { "Building Habits", 10 },    // 10 in a row (moderate - a few days)
            { "Consistency", 15 },        // 15 in a row (good - several days)
            { "Dedication", 25 },         // 25 in a row (impressive - ~1 week)
            { "Commitment", 50 },         // 50 in a row (great - ~2 weeks)
            { "Champion", 75 },           // 75 in a row (amazing - ~3 weeks)
            { "Legend", 100 }             // 100 in a row (legendary - ~1 month)
        };
        
        foreach (var badge in badgeDefinitions)
        {
            if (BestStreak >= badge.Value && !UnlockedBadges.Contains(badge.Key))
            {
                UnlockedBadges.Add(badge.Key);
                newBadges.Add(badge.Key);
            }
        }
        
        return newBadges;
    }
}
