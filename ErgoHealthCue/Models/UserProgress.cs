namespace ErgoHealthCue.Models;

public class UserProgress
{
    public int Level { get; set; } = 1;
    public int CurrentXP { get; set; } = 0;
    public int TotalXPEarned { get; set; } = 0;
    
    // Streak tracking
    public int CurrentStreak { get; set; } = 0;
    public int BestStreak { get; set; } = 0;
    public int CurrentNegativeStreak { get; set; } = 0;
    public int WorstNegativeStreak { get; set; } = 0;
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
        // Reset negative streak when completing a cue
        CurrentNegativeStreak = 0;
    }
    
    public void BreakStreak()
    {
        CurrentStreak = 0;
        CurrentNegativeStreak++;
        if (CurrentNegativeStreak > WorstNegativeStreak)
        {
            WorstNegativeStreak = CurrentNegativeStreak;
        }
    }
    
    public List<string> CheckAndUnlockBadges()
    {
        var newBadges = new List<string>();
        
        // 100 badge definitions - comprehensive progression system
        // At ~10 cues/day, spans from 1 day to ~3 years
        var badgeDefinitions = new Dictionary<string, int>
        {
            // Early achievements (1-20): First week
            { "First Step", 1 }, { "Baby Steps", 2 }, { "Getting Going", 3 }, { "Early Bird", 5 },
            { "Starter", 7 }, { "Beginner", 10 }, { "Novice", 15 }, { "Learner", 20 },
            { "Apprentice", 25 }, { "Student", 30 }, { "Trainee", 35 }, { "Freshman", 40 },
            { "Rookie", 45 }, { "Newbie", 50 }, { "Initiate", 60 }, { "Explorer", 70 },
            { "Adventurer", 80 }, { "Discoverer", 90 }, { "Pioneer", 100 }, { "Trailblazer", 110 },
            
            // Building momentum (21-40): 2-4 weeks
            { "Go-Getter", 125 }, { "Achiever", 140 }, { "Striver", 155 }, { "Worker", 170 },
            { "Grinder", 185 }, { "Hustler", 200 }, { "Climber", 220 }, { "Progressor", 240 },
            { "Developer", 260 }, { "Builder", 280 }, { "Creator", 300 }, { "Maker", 325 },
            { "Shaper", 350 }, { "Former", 375 }, { "Establisher", 400 }, { "Founder", 425 },
            { "Settler", 450 }, { "Organizer", 475 }, { "Planner", 500 }, { "Strategist", 530 },
            
            // Consistent effort (41-60): 2-3 months
            { "Tactician", 560 }, { "Coordinator", 590 }, { "Director", 620 }, { "Manager", 650 },
            { "Leader", 680 }, { "Commander", 710 }, { "Chief", 740 }, { "Captain", 770 },
            { "Major", 800 }, { "Colonel", 835 }, { "General", 870 }, { "Marshal", 905 },
            { "Warden", 940 }, { "Guardian", 975 }, { "Protector", 1010 }, { "Defender", 1050 },
            { "Champion", 1090 }, { "Hero", 1130 }, { "Warrior", 1170 }, { "Fighter", 1210 },
            
            // Advanced dedication (61-80): 4-6 months
            { "Gladiator", 1260 }, { "Samurai", 1310 }, { "Ninja", 1360 }, { "Monk", 1410 },
            { "Sensei", 1460 }, { "Master", 1520 }, { "Expert", 1580 }, { "Specialist", 1640 },
            { "Professional", 1700 }, { "Authority", 1770 }, { "Virtuoso", 1840 }, { "Ace", 1910 },
            { "Star", 1980 }, { "Icon", 2050 }, { "Celebrity", 2130 }, { "Superstar", 2210 },
            { "Phenomenon", 2290 }, { "Prodigy", 2370 }, { "Genius", 2450 }, { "Mastermind", 2540 },
            
            // Elite tier (81-100): 7 months to 3+ years
            { "Sage", 2630 }, { "Wizard", 2720 }, { "Sorcerer", 2810 }, { "Magician", 2900 },
            { "Enchanter", 3000 }, { "Oracle", 3120 }, { "Prophet", 3240 }, { "Mystic", 3360 },
            { "Titan", 3500 }, { "Colossus", 3650 }, { "Behemoth", 3800 }, { "Leviathan", 3950 },
            { "Immortal", 4120 }, { "Eternal", 4290 }, { "Infinite", 4460 }, { "Cosmic", 4650 },
            { "Celestial", 4840 }, { "Divine", 5030 }, { "Transcendent", 5250 }, { "Legendary", 10000 }
        };
        
        foreach (var badge in badgeDefinitions)
        {
            if (CurrentStreak >= badge.Value && !UnlockedBadges.Contains(badge.Key))
            {
                UnlockedBadges.Add(badge.Key);
                newBadges.Add(badge.Key);
            }
        }
        
        // Comical negative badges based on dismissals
        var negativeBadgeDefinitions = new Dictionary<string, int>
        {
            { "Rebel 😏", 3 },
            { "Procrastinator 🙄", 5 },
            { "Dismissive 😒", 10 },
            { "Too Busy? 🤔", 15 },
            { "Health Denier 😤", 20 },
            { "Couch Commander 🛋️", 25 },
            { "Button Masher 🖱️", 30 },
            { "Chronic Clicker 💢", 40 },
            { "Ergonomic Anarchist 🏴", 50 },
            { "Professional Ignorer 🙈", 75 },
            { "Master Avoider 🏃", 100 },
            { "Stubborn Sitter 🪑", 150 },
            { "Back Pain Collector 💀", 200 }
        };
        
        foreach (var badge in negativeBadgeDefinitions)
        {
            if (WorstNegativeStreak >= badge.Value && !UnlockedBadges.Contains(badge.Key))
            {
                UnlockedBadges.Add(badge.Key);
                newBadges.Add(badge.Key);
            }
        }
        
        return newBadges;
    }
}
