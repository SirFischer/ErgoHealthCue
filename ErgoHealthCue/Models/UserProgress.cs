namespace ErgoHealthCue.Models;

public class UserProgress
{
    public int Level { get; set; } = 1;
    public int CurrentXP { get; set; } = 0;
    public int TotalXPEarned { get; set; } = 0;
    
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
}
