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
        if (_allStatistics == null) return;
        
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
}
