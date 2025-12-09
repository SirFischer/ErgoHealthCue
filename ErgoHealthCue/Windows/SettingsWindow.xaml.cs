using System.Collections.ObjectModel;
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
        MinExerciseIntervalTextBox.Text = _settings.MinExerciseIntervalMinutes.ToString();
        MaxExerciseIntervalTextBox.Text = _settings.MaxExerciseIntervalMinutes.ToString();
        
        RandomPositionIntervalsCheckBox.IsChecked = _settings.UseRandomPositionIntervals;
        MinPositionIntervalTextBox.Text = _settings.MinPositionIntervalMinutes.ToString();
        MaxPositionIntervalTextBox.Text = _settings.MaxPositionIntervalMinutes.ToString();
        
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
        
        // Setup cues data grid with filtering
        _allCues = new ObservableCollection<Cue>(_settings.Cues);
        _filteredCues = new ObservableCollection<Cue>();
        CuesDataGrid.ItemsSource = _filteredCues;
        
        // Initial filter
        FilterCues();
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

    private void ViewStatsButton_Click(object sender, RoutedEventArgs e)
    {
        var statsWindow = new StatisticsWindow(_dataService);
        statsWindow.ShowDialog();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate exercise intervals
        if (!int.TryParse(MinExerciseIntervalTextBox.Text, out int minExerciseInterval) || minExerciseInterval < 1)
        {
            MessageBox.Show("Please enter a valid minimum exercise interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        if (!int.TryParse(MaxExerciseIntervalTextBox.Text, out int maxExerciseInterval) || maxExerciseInterval < minExerciseInterval)
        {
            MessageBox.Show("Maximum exercise interval must be greater than or equal to minimum interval.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        // Validate position intervals
        if (!int.TryParse(MinPositionIntervalTextBox.Text, out int minPositionInterval) || minPositionInterval < 1)
        {
            MessageBox.Show("Please enter a valid minimum position interval (at least 1 minute).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        if (!int.TryParse(MaxPositionIntervalTextBox.Text, out int maxPositionInterval) || maxPositionInterval < minPositionInterval)
        {
            MessageBox.Show("Maximum position interval must be greater than or equal to minimum interval.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        // Update settings
        _settings.UseRandomExerciseIntervals = RandomExerciseIntervalsCheckBox.IsChecked ?? true;
        _settings.MinExerciseIntervalMinutes = minExerciseInterval;
        _settings.MaxExerciseIntervalMinutes = maxExerciseInterval;
        
        _settings.UseRandomPositionIntervals = RandomPositionIntervalsCheckBox.IsChecked ?? true;
        _settings.MinPositionIntervalMinutes = minPositionInterval;
        _settings.MaxPositionIntervalMinutes = maxPositionInterval;
        
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
