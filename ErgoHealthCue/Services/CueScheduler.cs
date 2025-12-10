using System.Windows.Threading;
using ErgoHealthCue.Models;

namespace ErgoHealthCue.Services;

public class CueScheduler
{
    private readonly DispatcherTimer _exerciseTimer;
    private readonly DispatcherTimer _positionTimer;
    private readonly DispatcherTimer? _pauseTimer;
    private AppSettings _settings;
    private readonly DataService _dataService;
    private Guid? _lastExerciseCueId;
    private Guid? _lastPositionCueId;
    private bool _isPaused;
    private DateTime _exerciseTimerStartTime;
    private DateTime _positionTimerStartTime;
    private TimeSpan _exerciseTimerInterval;
    private TimeSpan _positionTimerInterval;
    
    private static readonly HashSet<CueType> PositionChangeCueTypes = new()
    {
        CueType.DeskStanding,
        CueType.DeskSitting,
        CueType.DeskFloor
    };

    public event EventHandler<Cue>? CueTriggered;
    public event EventHandler? PauseEnded;
    public bool IsPaused => _isPaused;
    
    // Properties to expose timer state
    public TimeSpan ExerciseTimeRemaining 
    {
        get
        {
            if (!_exerciseTimer.IsEnabled || _isPaused)
                return TimeSpan.Zero;
            
            var elapsed = DateTime.Now - _exerciseTimerStartTime;
            var remaining = _exerciseTimerInterval - elapsed;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
    
    public TimeSpan PositionTimeRemaining 
    {
        get
        {
            if (!_positionTimer.IsEnabled || _isPaused)
                return TimeSpan.Zero;
            
            var elapsed = DateTime.Now - _positionTimerStartTime;
            var remaining = _positionTimerInterval - elapsed;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
    
    public TimeSpan ExerciseTimerInterval => _exerciseTimerInterval;
    public TimeSpan PositionTimerInterval => _positionTimerInterval;

    public CueScheduler(AppSettings settings, DataService dataService)
    {
        _settings = settings;
        _dataService = dataService;
        
        _exerciseTimer = new DispatcherTimer();
        _exerciseTimer.Tick += ExerciseTimer_Tick;
        
        _positionTimer = new DispatcherTimer();
        _positionTimer.Tick += PositionTimer_Tick;
        
        _pauseTimer = new DispatcherTimer();
        _pauseTimer.Tick += PauseTimer_Tick;
    }

    public void Start()
    {
        ScheduleNextExerciseCue();
        ScheduleNextPositionCue();
        _exerciseTimerStartTime = DateTime.Now;
        _positionTimerStartTime = DateTime.Now;
        _exerciseTimer.Start();
        _positionTimer.Start();
    }

    public void Stop()
    {
        _exerciseTimer.Stop();
        _positionTimer.Stop();
    }

    public void Pause()
    {
        _isPaused = true;
        _exerciseTimer.Stop();
        _positionTimer.Stop();
        _pauseTimer?.Stop();
    }
    
    public void Pause(TimeSpan duration)
    {
        _isPaused = true;
        _exerciseTimer.Stop();
        _positionTimer.Stop();
        
        // Set up auto-resume timer
        _pauseTimer!.Interval = duration;
        _pauseTimer.Start();
    }

    public void Resume()
    {
        _isPaused = false;
        _pauseTimer?.Stop();
        _exerciseTimerStartTime = DateTime.Now;
        _positionTimerStartTime = DateTime.Now;
        _exerciseTimer.Start();
        _positionTimer.Start();
    }
    
    private void PauseTimer_Tick(object? sender, EventArgs e)
    {
        _pauseTimer?.Stop();
        Resume();
        PauseEnded?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
        if (_exerciseTimer.IsEnabled || _positionTimer.IsEnabled)
        {
            Stop();
            Start();
        }
    }

    public void TriggerNow(bool isManual = false)
    {
        // Trigger an exercise cue immediately
        TriggerExerciseCue(isManual);
    }

    private void ScheduleNextExerciseCue()
    {
        int intervalMinutes;
        
        if (_settings.UseRandomExerciseIntervals)
        {
            intervalMinutes = Random.Shared.Next(_settings.MinExerciseIntervalMinutes, _settings.MaxExerciseIntervalMinutes + 1);
        }
        else
        {
            intervalMinutes = _settings.MinExerciseIntervalMinutes;
        }

        _exerciseTimerInterval = TimeSpan.FromMinutes(intervalMinutes);
        _exerciseTimer.Interval = _exerciseTimerInterval;
    }

    private void ScheduleNextPositionCue()
    {
        int intervalMinutes;
        
        if (_settings.UseRandomPositionIntervals)
        {
            intervalMinutes = Random.Shared.Next(_settings.MinPositionIntervalMinutes, _settings.MaxPositionIntervalMinutes + 1);
        }
        else
        {
            intervalMinutes = _settings.MinPositionIntervalMinutes;
        }

        _positionTimerInterval = TimeSpan.FromMinutes(intervalMinutes);
        _positionTimer.Interval = _positionTimerInterval;
    }

    private void ExerciseTimer_Tick(object? sender, EventArgs e)
    {
        TriggerExerciseCue(false);
        ScheduleNextExerciseCue();
        _exerciseTimerStartTime = DateTime.Now;
    }

    private void PositionTimer_Tick(object? sender, EventArgs e)
    {
        TriggerPositionCue();
        ScheduleNextPositionCue();
        _positionTimerStartTime = DateTime.Now;
    }

    private void TriggerExerciseCue(bool isManual = false)
    {
        var enabledCues = _settings.Cues.Where(c => c.IsEnabled && !PositionChangeCueTypes.Contains(c.Type)).ToList();
        
        if (enabledCues.Count == 0)
            return;

        // Exclude the last exercise cue to prevent repetition
        if (_lastExerciseCueId.HasValue)
        {
            enabledCues = enabledCues.Where(c => c.Id != _lastExerciseCueId.Value).ToList();
        }
        
        // If we only have one cue and it was just shown, we have to show it again
        if (enabledCues.Count == 0)
        {
            enabledCues = _settings.Cues.Where(c => c.IsEnabled && !PositionChangeCueTypes.Contains(c.Type)).ToList();
        }

        // Select exercise appropriate for current position
        var selectedCue = SelectExerciseForCurrentPosition(enabledCues);

        if (selectedCue != null)
        {
            _lastExerciseCueId = selectedCue.Id;
            // Pass isManual flag through event args or create a new event signature
            // For now, we'll use a workaround by adding a property to Cue temporarily
            CueTriggeredManually?.Invoke(this, (selectedCue, isManual));
        }
    }

    public event EventHandler<(Cue cue, bool isManual)>? CueTriggeredManually;

    private void TriggerPositionCue()
    {
        var enabledCues = _settings.Cues.Where(c => c.IsEnabled && PositionChangeCueTypes.Contains(c.Type)).ToList();
        
        // Filter based on position availability
        enabledCues = enabledCues.Where(c =>
        {
            return c.Type switch
            {
                CueType.DeskStanding => _settings.StandingPositionAvailable,
                CueType.DeskSitting => _settings.SittingPositionAvailable,
                CueType.DeskFloor => _settings.FloorPositionAvailable,
                _ => true
            };
        }).ToList();
        
        if (enabledCues.Count == 0)
            return;

        // Exclude the last position cue to prevent repetition
        if (_lastPositionCueId.HasValue)
        {
            enabledCues = enabledCues.Where(c => c.Id != _lastPositionCueId.Value).ToList();
        }
        
        // If we only have one cue and it was just shown, we have to show it again
        if (enabledCues.Count == 0)
        {
            enabledCues = _settings.Cues.Where(c => c.IsEnabled && PositionChangeCueTypes.Contains(c.Type)).ToList();
            enabledCues = enabledCues.Where(c =>
            {
                return c.Type switch
                {
                    CueType.DeskStanding => _settings.StandingPositionAvailable,
                    CueType.DeskSitting => _settings.SittingPositionAvailable,
                    CueType.DeskFloor => _settings.FloorPositionAvailable,
                    _ => true
                };
            }).ToList();
        }

        if (enabledCues.Count > 0)
        {
            var selectedCue = enabledCues[Random.Shared.Next(enabledCues.Count)];
            _lastPositionCueId = selectedCue.Id;
            CueTriggered?.Invoke(this, selectedCue);
        }
    }

    private Cue SelectExerciseForCurrentPosition(List<Cue> exerciseCues)
    {
        // Filter exercises based on current desk position and availability
        var appropriateCues = _settings.CurrentPosition switch
        {
            DeskPosition.Standing when _settings.StandingPositionAvailable => exerciseCues.Where(c => 
                c.Type == CueType.StandingStretch || 
                c.Type == CueType.StandingMobilityDrill).ToList(),
            DeskPosition.Sitting when _settings.SittingPositionAvailable => exerciseCues.Where(c => 
                c.Type == CueType.SittingStretch || 
                c.Type == CueType.SittingMobilityDrill).ToList(),
            DeskPosition.Floor when _settings.FloorPositionAvailable => exerciseCues.Where(c => 
                c.Type == CueType.FloorStretch || 
                c.Type == CueType.FloorMobilityDrill).ToList(),
            _ => exerciseCues
        };

        if (appropriateCues.Count == 0)
            appropriateCues = exerciseCues;

        return appropriateCues[Random.Shared.Next(appropriateCues.Count)];
    }

    public void UpdatePosition(DeskPosition newPosition)
    {
        _settings.CurrentPosition = newPosition;
        _dataService.SaveSettings(_settings);
    }
}
