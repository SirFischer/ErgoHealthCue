using System.Windows.Threading;
using ErgoHealthCue.Models;

namespace ErgoHealthCue.Services;

public class CueScheduler
{
    private readonly DispatcherTimer _timer;
    private readonly Random _random = new();
    private AppSettings _settings;
    private readonly DataService _dataService;

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
        var enabledCues = _settings.Cues.Where(c => c.IsEnabled).ToList();
        
        if (enabledCues.Count > 0)
        {
            var randomCue = enabledCues[_random.Next(enabledCues.Count)];
            CueTriggered?.Invoke(this, randomCue);
        }

        ScheduleNextCue();
    }
}
