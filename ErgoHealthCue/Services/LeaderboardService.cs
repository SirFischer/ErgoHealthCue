using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ErgoHealthCue.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace ErgoHealthCue.Services;

public class LeaderboardService
{
    private readonly FirebaseClient _firebaseClient;
    private const string FIREBASE_URL = "https://ergohealthcue-default-rtdb.europe-west1.firebasedatabase.app/";
    private readonly string _userId;
    private readonly string _username;
    private bool _isEnabled;
    
    public LeaderboardService(string userId, string username, bool isEnabled)
    {
        _userId = userId;
        _username = username;
        _isEnabled = isEnabled;
        _firebaseClient = new FirebaseClient(FIREBASE_URL);
    }
    
    public void UpdateEnabled(bool enabled)
    {
        _isEnabled = enabled;
    }
    
    public async Task UpdateLeaderboardAsync(UserProgress progress, int completedCues, int dismissedCues)
    {
        if (!_isEnabled || string.IsNullOrWhiteSpace(_username))
        {
            return;
        }
        
        try
        {
            var entry = new LeaderboardEntry
            {
                UserId = _userId,
                Username = _username,
                Level = progress.Level,
                TotalXP = progress.TotalXPEarned,
                CompletedCues = completedCues,
                DismissedCues = dismissedCues,
                LastUpdated = DateTime.UtcNow
            };
            
            await _firebaseClient
                .Child("leaderboard")
                .Child(_userId)
                .PutAsync(entry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to update leaderboard: {ex.Message}");
            // Silently fail - don't block the app if leaderboard is unavailable
        }
    }
    
    public async Task<List<LeaderboardEntry>> GetTopEntriesAsync(int count = 100)
    {
        if (!_isEnabled)
        {
            return new List<LeaderboardEntry>();
        }
        
        try
        {
            var entries = await _firebaseClient
                .Child("leaderboard")
                .OnceAsync<LeaderboardEntry>();
            
            return entries
                .Select(item => item.Object)
                .OrderByDescending(e => e.Level)
                .ThenByDescending(e => e.TotalXP)
                .Take(count)
                .ToList();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to fetch leaderboard: {ex.Message}");
            return new List<LeaderboardEntry>();
        }
    }
    
    public async Task<int> GetUserRankAsync()
    {
        if (!_isEnabled)
        {
            return -1;
        }
        
        try
        {
            var entries = await _firebaseClient
                .Child("leaderboard")
                .OnceAsync<LeaderboardEntry>();
            
            var sorted = entries
                .Select(item => item.Object)
                .OrderByDescending(e => e.Level)
                .ThenByDescending(e => e.TotalXP)
                .ToList();
            
            var userRank = sorted.FindIndex(e => e.UserId == _userId);
            return userRank >= 0 ? userRank + 1 : -1;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to get user rank: {ex.Message}");
            return -1;
        }
    }
}
