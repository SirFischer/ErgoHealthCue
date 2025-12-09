using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;

namespace ErgoHealthCue.Windows;

public class StatisticsViewModel
{
    public DateTime ShownAt { get; set; }
    public CueType CueType { get; set; }
    public string CueTitle { get; set; } = string.Empty;
    public bool WasCompleted { get; set; }
    public string ResponseTime { get; set; } = string.Empty;
}

public class StatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Completed" : "Dismissed";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public partial class StatisticsWindow : Window
{
    private readonly DataService _dataService;
    private List<CueStatistic> _allStatistics;

    public StatisticsWindow(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        _allStatistics = _dataService.LoadStatistics();
        LoadStatistics();
    }

    private void TimePeriodRadio_Checked(object sender, RoutedEventArgs e)
    {
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        if (_allStatistics == null || TotalCuesText == null) return;
        
        // Load user progress
        var settings = _dataService.LoadSettings();
        
        // Update level and progress with color
        LevelText.Text = settings.Progress.Level.ToString();
        LevelText.Foreground = new System.Windows.Media.SolidColorBrush(GetLevelColor(settings.Progress.Level));
        ProgressText.Text = $"{settings.Progress.GetProgressPercentage()}%";
        XPText.Text = $"{settings.Progress.CurrentXP} / {settings.Progress.XPForNextLevel()} XP";
        
        // Filter by selected time period
        var filteredStatistics = FilterByTimePeriod(_allStatistics);
        
        // Update summary
        TotalCuesText.Text = filteredStatistics.Count.ToString();
        CompletedText.Text = filteredStatistics.Count(s => s.WasCompleted).ToString();
        DismissedText.Text = filteredStatistics.Count(s => !s.WasCompleted).ToString();
        
        // Prepare data for grid
        var viewModels = filteredStatistics
            .OrderByDescending(s => s.ShownAt)
            .Select(s => new StatisticsViewModel
            {
                ShownAt = s.ShownAt,
                CueType = s.CueType,
                CueTitle = s.CueTitle,
                WasCompleted = s.WasCompleted,
                ResponseTime = GetResponseTime(s)
            })
            .ToList();
        
        StatisticsDataGrid.ItemsSource = viewModels;
    }

    private System.Windows.Media.Color GetLevelColor(int level)
    {
        // Color gradient: Green (1-20) -> Blue (21-40) -> Red (41-60) -> Purple (61-80) -> Gold (81-100)
        if (level <= 20)
        {
            // Green gradient
            byte intensity = (byte)(100 + (level * 7)); // 100-240
            return System.Windows.Media.Color.FromRgb(0, intensity, 0);
        }
        else if (level <= 40)
        {
            // Blue gradient
            int progress = level - 20;
            byte intensity = (byte)(100 + (progress * 7)); // 100-240
            return System.Windows.Media.Color.FromRgb(0, 0, intensity);
        }
        else if (level <= 60)
        {
            // Red gradient
            int progress = level - 40;
            byte intensity = (byte)(150 + (progress * 5)); // 150-250
            return System.Windows.Media.Color.FromRgb(intensity, 0, 0);
        }
        else if (level <= 80)
        {
            // Purple gradient
            int progress = level - 60;
            byte intensity = (byte)(120 + (progress * 6)); // 120-240
            return System.Windows.Media.Color.FromRgb(intensity, 0, intensity);
        }
        else
        {
            // Gold gradient (81-100)
            return System.Windows.Media.Color.FromRgb(255, 215, 0); // Gold
        }
    }

    private List<CueStatistic> FilterByTimePeriod(List<CueStatistic> statistics)
    {
        var now = DateTime.Now;
        
        if (TodayRadio?.IsChecked == true)
        {
            var startOfDay = now.Date;
            return statistics.Where(s => s.ShownAt >= startOfDay).ToList();
        }
        else if (WeekRadio?.IsChecked == true)
        {
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
            return statistics.Where(s => s.ShownAt >= startOfWeek).ToList();
        }
        else if (MonthRadio?.IsChecked == true)
        {
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            return statistics.Where(s => s.ShownAt >= startOfMonth).ToList();
        }
        else if (YearRadio?.IsChecked == true)
        {
            var startOfYear = new DateTime(now.Year, 1, 1);
            return statistics.Where(s => s.ShownAt >= startOfYear).ToList();
        }
        else // All Time
        {
            return statistics;
        }
    }

    private string GetResponseTime(CueStatistic stat)
    {
        DateTime? responseTime = stat.WasCompleted ? stat.CompletedAt : stat.DismissedAt;
        
        if (responseTime.HasValue)
        {
            var duration = responseTime.Value - stat.ShownAt;
            if (duration.TotalMinutes < 1)
                return $"{duration.Seconds} seconds";
            else if (duration.TotalHours < 1)
                return $"{duration.Minutes} minutes";
            else
                return $"{duration.Hours} hours {duration.Minutes} minutes";
        }
        
        return "N/A";
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ResetStatisticsButton_Click(object sender, RoutedEventArgs e)
    {
        var result = System.Windows.MessageBox.Show(
            "Are you sure you want to permanently delete all statistics and reset your level?\n\nThis action cannot be undone.",
            "Reset Statistics & Level",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);
            
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            // Reset statistics
            _allStatistics.Clear();
            _dataService.SaveStatistics(_allStatistics);
            
            // Reset level and XP
            var settings = _dataService.LoadSettings();
            settings.Progress = new UserProgress(); // Reset to level 1, 0 XP
            _dataService.SaveSettings(settings);
            
            LoadStatistics();
            System.Windows.MessageBox.Show(
                "All statistics and level progress have been reset.",
                "Success",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
    }
}
