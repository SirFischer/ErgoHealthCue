using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;
using MessageBox = System.Windows.MessageBox;

namespace ErgoHealthCue.Windows;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    private readonly DataService _dataService;
    private readonly StartupService _startupService;
    private readonly ObservableCollection<Cue> _allCues;
    private readonly ObservableCollection<Cue> _filteredCues;

    public AppSettings UpdatedSettings => _settings;

    public SettingsWindow(AppSettings settings, DataService dataService, StartupService startupService)
    {
        InitializeComponent();
        
        _settings = settings;
        _dataService = dataService;
        _startupService = startupService;
        
        // Load settings to UI
        RandomExerciseIntervalsCheckBox.IsChecked = _settings.UseRandomExerciseIntervals;
        if (_settings.UseRandomExerciseIntervals)
        {
            MinExerciseIntervalTextBox.Text = _settings.MinExerciseIntervalMinutes.ToString();
            MaxExerciseIntervalTextBox.Text = _settings.MaxExerciseIntervalMinutes.ToString();
        }
        else
        {
            // When not using random, use MinInterval as the fixed interval
            FixedExerciseIntervalTextBox.Text = _settings.MinExerciseIntervalMinutes.ToString();
        }
        UpdateExerciseIntervalVisibility();
        
        RandomPositionIntervalsCheckBox.IsChecked = _settings.UseRandomPositionIntervals;
        if (_settings.UseRandomPositionIntervals)
        {
            MinPositionIntervalTextBox.Text = _settings.MinPositionIntervalMinutes.ToString();
            MaxPositionIntervalTextBox.Text = _settings.MaxPositionIntervalMinutes.ToString();
        }
        else
        {
            // When not using random, use MinInterval as the fixed interval
            FixedPositionIntervalTextBox.Text = _settings.MinPositionIntervalMinutes.ToString();
        }
        UpdatePositionIntervalVisibility();
        
        StartupCheckBox.IsChecked = _startupService.IsStartupEnabled();
        
        // Load language setting
        string language = _settings.Language ?? "auto";
        for (int i = 0; i < LanguageComboBox.Items.Count; i++)
        {
            if (LanguageComboBox.Items[i] is ComboBoxItem item && item.Tag?.ToString() == language)
            {
                LanguageComboBox.SelectedIndex = i;
                break;
            }
        }
        
        // Load leaderboard settings
        LeaderboardEnabledCheckBox.IsChecked = _settings.LeaderboardEnabled;
        UsernameTextBox.Text = _settings.Username;
        
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
        
        // Setup cues data grid with filtering
        _allCues = new ObservableCollection<Cue>(_settings.Cues);
        _filteredCues = new ObservableCollection<Cue>();
        CuesDataGrid.ItemsSource = _filteredCues;
        
        // Initial filter
        FilterCues();
    }

    private void RandomExerciseIntervalsCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        UpdateExerciseIntervalVisibility();
    }

    private void RandomPositionIntervalsCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        UpdatePositionIntervalVisibility();
    }

    private void UpdateExerciseIntervalVisibility()
    {
        if (RandomExerciseIntervalsCheckBox.IsChecked == true)
        {
            RandomExerciseIntervalPanel.Visibility = Visibility.Visible;
            FixedExerciseIntervalPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            RandomExerciseIntervalPanel.Visibility = Visibility.Collapsed;
            FixedExerciseIntervalPanel.Visibility = Visibility.Visible;
        }
    }

    private void UpdatePositionIntervalVisibility()
    {
        if (RandomPositionIntervalsCheckBox.IsChecked == true)
        {
            RandomPositionIntervalPanel.Visibility = Visibility.Visible;
            FixedPositionIntervalPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            RandomPositionIntervalPanel.Visibility = Visibility.Collapsed;
            FixedPositionIntervalPanel.Visibility = Visibility.Visible;
        }
    }

    private void PositionRadio_Checked(object sender, RoutedEventArgs e)
    {
        FilterCues();
    }

    private void FilterCues()
    {
        if (_filteredCues == null) return;
        
        _filteredCues.Clear();
        
        List<Cue> cues;
        string filterText;
        
        if (StandingRadio?.IsChecked == true)
        {
            cues = _allCues.Where(c => c.Type == CueType.StandingStretch || c.Type == CueType.StandingMobilityDrill).ToList();
            filterText = "Showing: Standing Position Cues";
        }
        else if (SittingRadio?.IsChecked == true)
        {
            cues = _allCues.Where(c => c.Type == CueType.SittingStretch || c.Type == CueType.SittingMobilityDrill).ToList();
            filterText = "Showing: Sitting Position Cues";
        }
        else if (FloorRadio?.IsChecked == true)
        {
            cues = _allCues.Where(c => c.Type == CueType.FloorStretch || c.Type == CueType.FloorMobilityDrill).ToList();
            filterText = "Showing: Floor Position Cues";
        }
        else if (PositionChangesRadio?.IsChecked == true)
        {
            cues = _allCues.Where(c => c.Type == CueType.DeskStanding || c.Type == CueType.DeskSitting || c.Type == CueType.DeskFloor).ToList();
            filterText = "Showing: Position Change Cues";
        }
        else
        {
            cues = _allCues.ToList();
            filterText = "Showing: All Cues";
        }
        
        foreach (var cue in cues)
        {
            _filteredCues.Add(cue);
        }
        
        if (CurrentFilterText != null)
        {
            CurrentFilterText.Text = filterText;
        }
    }

    private void AddCueButton_Click(object sender, RoutedEventArgs e)
    {
        // Determine the appropriate cue type based on current filter
        CueType defaultType = CueType.SittingStretch;
        
        if (StandingRadio?.IsChecked == true)
            defaultType = CueType.StandingStretch;
        else if (SittingRadio?.IsChecked == true)
            defaultType = CueType.SittingStretch;
        else if (FloorRadio?.IsChecked == true)
            defaultType = CueType.FloorStretch;
        else if (PositionChangesRadio?.IsChecked == true)
            defaultType = CueType.DeskStanding;
        
        var addCueWindow = new AddCueWindow(defaultType);
        if (addCueWindow.ShowDialog() == true)
        {
            _allCues.Add(addCueWindow.NewCue);
            FilterCues();
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
                _allCues.Remove(selectedCue);
                FilterCues();
            }
        }
        else
        {
            MessageBox.Show("Please select a cue to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ResetToDefaultButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "This will REPLACE all your cues with the factory default cues. All custom cues will be lost.\n\nDo you want to continue?",
            "Reset to Default Cues",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
            
        if (result == MessageBoxResult.Yes)
        {
            _allCues.Clear();
            var defaultCues = _dataService.GetDefaultCues();
            
            foreach (var cue in defaultCues)
            {
                _allCues.Add(cue);
            }
            
            FilterCues();
            MessageBox.Show($"Reset to {defaultCues.Count} default cues.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ViewStatsButton_Click(object sender, RoutedEventArgs e)
    {
        var statsWindow = new StatisticsWindow(_dataService);
        statsWindow.ShowDialog();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        int minExerciseInterval, maxExerciseInterval;
        int minPositionInterval, maxPositionInterval;
        
        // Handle exercise intervals
        if (RandomExerciseIntervalsCheckBox.IsChecked == true)
        {
            // Validate random exercise intervals
            if (!int.TryParse(MinExerciseIntervalTextBox.Text, out minExerciseInterval) || minExerciseInterval < 1)
            {
                MessageBox.Show("Please enter a valid minimum exercise interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!int.TryParse(MaxExerciseIntervalTextBox.Text, out maxExerciseInterval) || maxExerciseInterval < minExerciseInterval)
            {
                MessageBox.Show("Maximum exercise interval must be greater than or equal to minimum interval.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        else
        {
            // Validate fixed exercise interval
            if (!int.TryParse(FixedExerciseIntervalTextBox.Text, out int fixedExerciseInterval) || fixedExerciseInterval < 1)
            {
                MessageBox.Show("Please enter a valid exercise interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // For fixed intervals, set both min and max to the same value
            minExerciseInterval = fixedExerciseInterval;
            maxExerciseInterval = fixedExerciseInterval;
        }
        
        // Handle position intervals
        if (RandomPositionIntervalsCheckBox.IsChecked == true)
        {
            // Validate random position intervals
            if (!int.TryParse(MinPositionIntervalTextBox.Text, out minPositionInterval) || minPositionInterval < 1)
            {
                MessageBox.Show("Please enter a valid minimum position interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!int.TryParse(MaxPositionIntervalTextBox.Text, out maxPositionInterval) || maxPositionInterval < minPositionInterval)
            {
                MessageBox.Show("Maximum position interval must be greater than or equal to minimum interval.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        else
        {
            // Validate fixed position interval
            if (!int.TryParse(FixedPositionIntervalTextBox.Text, out int fixedPositionInterval) || fixedPositionInterval < 1)
            {
                MessageBox.Show("Please enter a valid position interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // For fixed intervals, set both min and max to the same value
            minPositionInterval = fixedPositionInterval;
            maxPositionInterval = fixedPositionInterval;
        }
        
        // Update settings
        _settings.UseRandomExerciseIntervals = RandomExerciseIntervalsCheckBox.IsChecked ?? true;
        _settings.MinExerciseIntervalMinutes = minExerciseInterval;
        _settings.MaxExerciseIntervalMinutes = maxExerciseInterval;
        
        _settings.UseRandomPositionIntervals = RandomPositionIntervalsCheckBox.IsChecked ?? true;
        _settings.MinPositionIntervalMinutes = minPositionInterval;
        _settings.MaxPositionIntervalMinutes = maxPositionInterval;
        
        // Save language preference
        if (LanguageComboBox.SelectedItem is ComboBoxItem selectedLanguage)
        {
            _settings.Language = selectedLanguage.Tag?.ToString() ?? "auto";
        }
        
        _settings.Cues = _allCues.ToList();
        
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
        
        // Save leaderboard settings
        _settings.LeaderboardEnabled = LeaderboardEnabledCheckBox.IsChecked ?? false;
        _settings.Username = UsernameTextBox.Text?.Trim() ?? string.Empty;
        
        // Validate username if leaderboard is enabled
        if (_settings.LeaderboardEnabled && string.IsNullOrWhiteSpace(_settings.Username))
        {
            MessageBox.Show("Please enter a username to enable the leaderboard.", "Username Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
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
    
    private void ViewLeaderboardButton_Click(object sender, RoutedEventArgs e)
    {
        // Create a temporary leaderboard service to view leaderboard
        var leaderboardService = new LeaderboardService(
            _settings.UserId,
            _settings.Username,
            _settings.LeaderboardEnabled
        );
        
        var leaderboardWindow = new LeaderboardWindow(leaderboardService, _settings);
        leaderboardWindow.ShowDialog();
    }
}
