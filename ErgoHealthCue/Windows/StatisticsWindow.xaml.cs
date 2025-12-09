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

    public StatisticsWindow(DataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        var statistics = _dataService.LoadStatistics();
        
        // Update summary
        TotalCuesText.Text = statistics.Count.ToString();
        CompletedText.Text = statistics.Count(s => s.WasCompleted).ToString();
        DismissedText.Text = statistics.Count(s => !s.WasCompleted).ToString();
        
        // Prepare data for grid
        var viewModels = statistics
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
