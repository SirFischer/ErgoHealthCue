using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;
using MessageBox = System.Windows.MessageBox;

namespace ErgoHealthCue.Windows;

public partial class LeaderboardWindow : Window
{
    private readonly LeaderboardService _leaderboardService;
    private readonly AppSettings _settings;
    
    public LeaderboardWindow(LeaderboardService leaderboardService, AppSettings settings)
    {
        InitializeComponent();
        _leaderboardService = leaderboardService;
        _settings = settings;
        
        Loaded += async (s, e) => await LoadLeaderboardAsync();
    }
    
    private async Task LoadLeaderboardAsync()
    {
        try
        {
            // Show loading state
            LeaderboardGrid.IsEnabled = false;
            
            // Load leaderboard entries
            var entries = await _leaderboardService.GetTopEntriesAsync(100);
            
            // Add rank to entries
            var rankedEntries = entries.Select((entry, index) => new
            {
                Rank = index + 1,
                entry.Username,
                entry.Level,
                entry.TotalXP,
                entry.CompletedCues,
                entry.DismissedCues,
                entry.CompletionRate,
                entry.UserId
            }).ToList();
            
            LeaderboardGrid.ItemsSource = rankedEntries;
            
            // Update user stats
            var userEntry = rankedEntries.FirstOrDefault(e => e.UserId == _settings.UserId);
            if (userEntry != null)
            {
                RankText.Text = $"#{userEntry.Rank}";
                LevelText.Text = userEntry.Level.ToString();
                TotalXPText.Text = userEntry.TotalXP.ToString("N0");
                CompletionRateText.Text = $"{userEntry.CompletionRate:F1}%";
            }
            else
            {
                // Show current user stats even if not on leaderboard
                RankText.Text = "-";
                LevelText.Text = _settings.Progress.Level.ToString();
                TotalXPText.Text = _settings.Progress.TotalXPEarned.ToString("N0");
                CompletionRateText.Text = "-";
            }
            
            LeaderboardGrid.IsEnabled = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load leaderboard: {ex.Message}");
            MessageBox.Show("Unable to load the leaderboard. Please check your internet connection and try again.", 
                "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LeaderboardGrid.IsEnabled = true;
        }
    }
    
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadLeaderboardAsync();
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
