using System.Collections.ObjectModel;
using System.Windows;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;
using MessageBox = System.Windows.MessageBox;

namespace ErgoHealthCue.Windows;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    private readonly DataService _dataService;
    private readonly StartupService _startupService;
    private readonly ObservableCollection<Cue> _cues;

    public AppSettings UpdatedSettings => _settings;

    public SettingsWindow(AppSettings settings, DataService dataService, StartupService startupService)
    {
        InitializeComponent();
        
        _settings = settings;
        _dataService = dataService;
        _startupService = startupService;
        
        // Load settings to UI
        RandomIntervalsCheckBox.IsChecked = _settings.UseRandomIntervals;
        MinIntervalTextBox.Text = _settings.MinIntervalMinutes.ToString();
        MaxIntervalTextBox.Text = _settings.MaxIntervalMinutes.ToString();
        StartupCheckBox.IsChecked = _startupService.IsStartupEnabled();
        
        // Set current position
        switch (_settings.CurrentPosition)
        {
            case DeskPosition.Standing:
                StandingRadio.IsChecked = true;
                break;
            case DeskPosition.Sitting:
                SittingRadio.IsChecked = true;
                break;
            case DeskPosition.Floor:
                FloorRadio.IsChecked = true;
                break;
        }
        
        // Setup cues data grid
        _cues = new ObservableCollection<Cue>(_settings.Cues);
        CuesDataGrid.ItemsSource = _cues;
    }

    private void AddCueButton_Click(object sender, RoutedEventArgs e)
    {
        var addCueWindow = new AddCueWindow();
        if (addCueWindow.ShowDialog() == true)
        {
            _cues.Add(addCueWindow.NewCue);
        }
    }

    private void RemoveCueButton_Click(object sender, RoutedEventArgs e)
    {
        if (CuesDataGrid.SelectedItem is Cue selectedCue)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to remove '{selectedCue.Title}'?",
                "Confirm Removal",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                _cues.Remove(selectedCue);
            }
        }
        else
        {
            MessageBox.Show("Please select a cue to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ViewStatsButton_Click(object sender, RoutedEventArgs e)
    {
        var statsWindow = new StatisticsWindow(_dataService);
        statsWindow.ShowDialog();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate inputs
        if (!int.TryParse(MinIntervalTextBox.Text, out int minInterval) || minInterval < 1)
        {
            MessageBox.Show("Please enter a valid minimum interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        if (!int.TryParse(MaxIntervalTextBox.Text, out int maxInterval) || maxInterval < minInterval)
        {
            MessageBox.Show("Maximum interval must be greater than or equal to minimum interval.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        // Update settings
        _settings.UseRandomIntervals = RandomIntervalsCheckBox.IsChecked ?? true;
        _settings.MinIntervalMinutes = minInterval;
        _settings.MaxIntervalMinutes = maxInterval;
        _settings.Cues = _cues.ToList();
        
        // Update current position
        if (StandingRadio.IsChecked == true)
            _settings.CurrentPosition = DeskPosition.Standing;
        else if (SittingRadio.IsChecked == true)
            _settings.CurrentPosition = DeskPosition.Sitting;
        else if (FloorRadio.IsChecked == true)
            _settings.CurrentPosition = DeskPosition.Floor;
        
        // Handle startup setting
        bool startupEnabled = StartupCheckBox.IsChecked ?? false;
        _startupService.SetStartup(startupEnabled);
        
        // Save to disk
        _dataService.SaveSettings(_settings);
        
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
