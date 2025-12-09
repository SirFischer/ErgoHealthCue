using System.Windows.Threading;
using ErgoHealthCue.Models;

namespace ErgoHealthCue.Services;

public class CueScheduler
{
    private readonly DispatcherTimer _timer;
    private readonly Random _random = new();
    private AppSettings _settings;
    private readonly DataService _dataService;
    
    private static readonly CueType[] PositionChangeCueTypes = new[]
    {
        CueType.DeskStanding,
        CueType.DeskSitting,
        CueType.DeskFloor
    };

    public event EventHandler<Cue>? CueTriggered;

    public CueScheduler(AppSettings settings, DataService dataService)
    {
        _settings = settings;
        _dataService = dataService;
        _timer = new DispatcherTimer();
        _timer.Tick += Timer_Tick;
    }

    public void Start()
    {
        ScheduleNextCue();
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
        if (_timer.IsEnabled)
        {
            Stop();
            Start();
        }
    }

    public void TriggerNow()
    {
        // Stop the timer, trigger a cue, then reschedule
        _timer.Stop();
        SelectAndTriggerCue();
        ScheduleNextCue();
        _timer.Start();
    }

    private void ScheduleNextCue()
    {
        int intervalMinutes;
        
        if (_settings.UseRandomIntervals)
        {
            intervalMinutes = _random.Next(_settings.MinIntervalMinutes, _settings.MaxIntervalMinutes + 1);
        }
        else
        {
            intervalMinutes = _settings.MinIntervalMinutes;
        }

        _timer.Interval = TimeSpan.FromMinutes(intervalMinutes);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        SelectAndTriggerCue();
        ScheduleNextCue();
    }

    private void SelectAndTriggerCue()
    {
        var enabledCues = _settings.Cues.Where(c => c.IsEnabled).ToList();
        
        if (enabledCues.Count == 0)
            return;

        // Separate position changes from exercises using constant array
        var positionCues = enabledCues.Where(c => PositionChangeCueTypes.Contains(c.Type)).ToList();
        var exerciseCues = enabledCues.Where(c => !PositionChangeCueTypes.Contains(c.Type)).ToList();

        // 30% chance for position change, 70% for exercise
        Cue? selectedCue = null;
        
        if (positionCues.Count > 0 && exerciseCues.Count > 0)
        {
            if (_random.Next(100) < 30)
            {
                // Position change
                selectedCue = positionCues[_random.Next(positionCues.Count)];
            }
            else
            {
                // Exercise appropriate for current position
                selectedCue = SelectExerciseForCurrentPosition(exerciseCues);
            }
        }
        else if (positionCues.Count > 0)
        {
            selectedCue = positionCues[_random.Next(positionCues.Count)];
        }
        else if (exerciseCues.Count > 0)
        {
            selectedCue = SelectExerciseForCurrentPosition(exerciseCues);
        }

        if (selectedCue != null)
        {
            CueTriggered?.Invoke(this, selectedCue);
        }
    }

    private Cue SelectExerciseForCurrentPosition(List<Cue> exerciseCues)
    {
        // Filter exercises based on current desk position
        var appropriateCues = _settings.CurrentPosition switch
        {
            DeskPosition.Standing => exerciseCues.Where(c => 
                c.Type == CueType.StandingStretch || 
                c.Type == CueType.StandingMobilityDrill).ToList(),
            DeskPosition.Sitting => exerciseCues.Where(c => 
                c.Type == CueType.SittingStretch || 
                c.Type == CueType.SittingMobilityDrill).ToList(),
            DeskPosition.Floor => exerciseCues.Where(c => 
                c.Type == CueType.FloorStretch || 
                c.Type == CueType.FloorMobilityDrill).ToList(),
            _ => exerciseCues
        };

        if (appropriateCues.Count == 0)
            appropriateCues = exerciseCues;

        return appropriateCues[_random.Next(appropriateCues.Count)];
    }

    public void UpdatePosition(DeskPosition newPosition)
    {
        _settings.CurrentPosition = newPosition;
        _dataService.SaveSettings(_settings);
    }
}
