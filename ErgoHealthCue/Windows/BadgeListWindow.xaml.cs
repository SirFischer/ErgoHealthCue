using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ErgoHealthCue.Models;

namespace ErgoHealthCue.Windows;

public class BadgeViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string RequirementText { get; set; } = string.Empty;
    public bool IsUnlocked { get; set; }
    public bool IsNegative { get; set; }
    public int RequiredCount { get; set; }
    public Style? Style { get; set; }
    public System.Windows.Media.Brush NameColor { get; set; } = System.Windows.Media.Brushes.Black;
    public System.Windows.Media.Brush RequirementColor { get; set; } = System.Windows.Media.Brushes.Gray;
    public Visibility RequirementVisibility { get; set; } = Visibility.Visible;
    public string Tooltip { get; set; } = string.Empty;
}

public partial class BadgeListWindow : Window
{
    private readonly UserProgress _userProgress;
    private List<BadgeViewModel> _allBadges = new();

    public BadgeListWindow(UserProgress userProgress)
    {
        InitializeComponent();
        _userProgress = userProgress;
        LoadBadges();
        UpdateDisplay();
    }

    private void LoadBadges()
    {
        _allBadges = new List<BadgeViewModel>();

        // Load positive badges from UserProgress.CheckAndUnlockBadges
        var positiveBadgeDefinitions = new Dictionary<string, int>
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

        foreach (var badge in positiveBadgeDefinitions.OrderBy(b => b.Value))
        {
            bool isUnlocked = _userProgress.UnlockedBadges.Contains(badge.Key);
            _allBadges.Add(new BadgeViewModel
            {
                Name = badge.Key,
                Icon = isUnlocked ? "🏆" : "🔒",
                RequirementText = isUnlocked ? $"Complete {badge.Value} cues in a row" : "???",
                IsUnlocked = isUnlocked,
                IsNegative = false,
                RequiredCount = badge.Value,
                Style = (Style)FindResource(isUnlocked ? "BadgeCardUnlocked" : "BadgeCardLocked"),
                NameColor = new SolidColorBrush(isUnlocked ? System.Windows.Media.Color.FromRgb(245, 158, 11) : System.Windows.Media.Color.FromRgb(156, 163, 175)),
                RequirementColor = new SolidColorBrush(isUnlocked ? System.Windows.Media.Color.FromRgb(107, 114, 128) : System.Windows.Media.Color.FromRgb(156, 163, 175)),
                RequirementVisibility = Visibility.Visible,
                Tooltip = isUnlocked ? $"Unlocked by completing {badge.Value} cues in a row" : "Complete more cues to unlock this badge"
            });
        }

        // Load negative badges
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

        foreach (var badge in negativeBadgeDefinitions.OrderBy(b => b.Value))
        {
            bool isUnlocked = _userProgress.UnlockedBadges.Contains(badge.Key);
            _allBadges.Add(new BadgeViewModel
            {
                Name = badge.Key,
                Icon = isUnlocked ? "💩" : "🔒",
                RequirementText = isUnlocked ? $"Dismiss {badge.Value} cues in a row" : "???",
                IsUnlocked = isUnlocked,
                IsNegative = true,
                RequiredCount = badge.Value,
                Style = (Style)FindResource(isUnlocked ? "BadgeCardNegative" : "BadgeCardLocked"),
                NameColor = new SolidColorBrush(isUnlocked ? System.Windows.Media.Color.FromRgb(239, 68, 68) : System.Windows.Media.Color.FromRgb(156, 163, 175)),
                RequirementColor = new SolidColorBrush(isUnlocked ? System.Windows.Media.Color.FromRgb(107, 114, 128) : System.Windows.Media.Color.FromRgb(156, 163, 175)),
                RequirementVisibility = Visibility.Visible,
                Tooltip = isUnlocked ? $"Unlocked by dismissing {badge.Value} cues in a row" : "Keep dismissing cues to unlock (not recommended!)"
            });
        }

        // Update subtitle
        int unlockedCount = _allBadges.Count(b => b.IsUnlocked);
        SubtitleText.Text = $"{unlockedCount} / {_allBadges.Count} badges unlocked";
    }

    private void BadgeFilter_Checked(object sender, RoutedEventArgs e)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (BadgeItemsControl == null || _allBadges == null) return;

        List<BadgeViewModel> filteredBadges;

        if (PositiveBadgesRadio?.IsChecked == true)
        {
            filteredBadges = _allBadges.Where(b => !b.IsNegative).ToList();
        }
        else if (NegativeBadgesRadio?.IsChecked == true)
        {
            filteredBadges = _allBadges.Where(b => b.IsNegative).ToList();
        }
        else if (UnlockedOnlyRadio?.IsChecked == true)
        {
            filteredBadges = _allBadges.Where(b => b.IsUnlocked).ToList();
        }
        else // All Badges
        {
            filteredBadges = _allBadges.ToList();
        }

        BadgeItemsControl.ItemsSource = filteredBadges;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
