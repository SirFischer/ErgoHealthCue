using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;

namespace ErgoHealthCue.Windows;

public partial class CueOverlayWindow : Window
{
    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    private readonly Cue _cue;
    private readonly DataService _dataService;
    private readonly CueScheduler _scheduler;
    private readonly AppSettings _settings;
    private readonly CueStatistic _statistic;
    private readonly DispatcherTimer _flashTimer;
    private readonly DispatcherTimer _countdownTimer;
    private readonly XPCalculator _xpCalculator;
    private bool _isFlashOn = true;
    private int _remainingSeconds = 300; // 5 minutes
    private int _potentialXP;
    private int _penaltyXP;
    private bool _wasUserActive = true;
    private int _previousLevel;

    public CueOverlayWindow(Cue cue, DataService dataService, CueScheduler scheduler, AppSettings settings)
    {
        InitializeComponent();
        _cue = cue;
        _dataService = dataService;
        _scheduler = scheduler;
        _settings = settings;
        _xpCalculator = new XPCalculator();
        _previousLevel = settings.Progress.Level;

        TitleText.Text = cue.Title;
        DescriptionText.Text = cue.Description;

        // Calculate XP
        int avgInterval = IsPositionChangeCue() 
            ? (settings.MinPositionIntervalMinutes + settings.MaxPositionIntervalMinutes) / 2
            : (settings.MinExerciseIntervalMinutes + settings.MaxExerciseIntervalMinutes) / 2;
        
        _potentialXP = _xpCalculator.CalculateXPGain(cue.Type, avgInterval, true);
        _penaltyXP = Math.Abs(_xpCalculator.CalculateXPGain(cue.Type, avgInterval, false));
        
        XPText.Text = $"+{_potentialXP} XP";
        PenaltyText.Text = $"(-{_penaltyXP} XP if dismissed/timeout)";

        _statistic = new CueStatistic
        {
            CueId = cue.Id,
            CueTitle = cue.Title,
            CueType = cue.Type,
            ShownAt = DateTime.Now
        };

        // Setup flashing effect
        _flashTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
        _flashTimer.Tick += FlashTimer_Tick;
        _flashTimer.Start();

        // Setup countdown timer
        _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _countdownTimer.Tick += CountdownTimer_Tick;
        _countdownTimer.Start();
        UpdateTimerDisplay();

        // Play smooth animation on show
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
        var scaleTransform = new ScaleTransform(0.9, 0.9);
        MainBorder.RenderTransform = scaleTransform;
        MainBorder.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        
        var scaleUp = new DoubleAnimation(0.9, 1, TimeSpan.FromMilliseconds(400))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        BeginAnimation(OpacityProperty, fadeIn);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);
    }

    private bool IsPositionChangeCue()
    {
        return _cue.Type == CueType.DeskStanding || 
               _cue.Type == CueType.DeskSitting || 
               _cue.Type == CueType.DeskFloor;
    }

    private bool IsUserActiveOnSystem()
    {
        LASTINPUTINFO lastInput = new LASTINPUTINFO();
        lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);
        
        if (GetLastInputInfo(ref lastInput))
        {
            uint idleTime = (uint)Environment.TickCount - lastInput.dwTime;
            // Consider user inactive if no input for more than 5 minutes
            return idleTime < 300000;
        }
        
        return true; // Assume active if we can't check
    }

    private void CountdownTimer_Tick(object? sender, EventArgs e)
    {
        // Check if user is active (not logged out)
        bool isActive = IsUserActiveOnSystem();
        
        // Only decrease timer if user is active
        if (isActive)
        {
            if (!_wasUserActive)
            {
                // User just became active again
                _wasUserActive = true;
            }
            
            _remainingSeconds--;
            UpdateTimerDisplay();

            if (_remainingSeconds <= 0)
            {
                // Timeout - apply penalty
                HandleTimeout();
            }
        }
        else
        {
            if (_wasUserActive)
            {
                // User just became inactive - pause timer
                _wasUserActive = false;
            }
            // Don't decrease timer while user is away
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = _remainingSeconds / 60;
        int seconds = _remainingSeconds % 60;
        TimerText.Text = $"{minutes}:{seconds:D2}";
        
        // Update timer bar width
        double percentage = (double)_remainingSeconds / 300.0;
        TimerBar.Width = MainBorder.ActualWidth * percentage;
        
        // Change color as time runs out
        if (_remainingSeconds <= 60)
        {
            TimerBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(239, 68, 68)); // Red
        }
        else if (_remainingSeconds <= 120)
        {
            TimerBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(251, 146, 60)); // Orange
        }
    }

    private void FlashTimer_Tick(object? sender, EventArgs e)
    {
        _isFlashOn = !_isFlashOn;
        
        var color1 = System.Windows.Media.Color.FromRgb(99, 102, 241);   // Indigo-500
        var color2 = System.Windows.Media.Color.FromRgb(139, 92, 246);   // Violet-500
        
        MainBorder.BorderBrush = new SolidColorBrush(_isFlashOn ? color1 : color2);
        IconCircle.Fill = new SolidColorBrush(_isFlashOn ? color1 : color2);
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        _flashTimer.Stop();
        _countdownTimer.Stop();
        
        _statistic.CompletedAt = DateTime.Now;
        _statistic.WasCompleted = true;
        _dataService.AddStatistic(_statistic);
        
        // Award XP
        _settings.Progress.AddXP(_potentialXP);
        _dataService.SaveSettings(_settings);
        
        // Update desk position if this was a position change cue
        UpdateDeskPosition();
        
        // Check for level up
        if (_settings.Progress.Level > _previousLevel)
        {
            ShowLevelUpNotification();
        }
        
        Close();
    }

    private void DismissButton_Click(object sender, RoutedEventArgs e)
    {
        _flashTimer.Stop();
        _countdownTimer.Stop();
        
        _statistic.DismissedAt = DateTime.Now;
        _statistic.WasCompleted = false;
        _dataService.AddStatistic(_statistic);
        
        // Apply XP penalty
        _settings.Progress.RemoveXP(_penaltyXP);
        _dataService.SaveSettings(_settings);
        
        Close();
    }

    private void HandleTimeout()
    {
        _flashTimer.Stop();
        _countdownTimer.Stop();
        
        _statistic.DismissedAt = DateTime.Now;
        _statistic.WasCompleted = false;
        _dataService.AddStatistic(_statistic);
        
        // Apply XP penalty for timeout
        _settings.Progress.RemoveXP(_penaltyXP);
        _dataService.SaveSettings(_settings);
        
        System.Windows.MessageBox.Show(
            $"Time's up! You lost {_penaltyXP} XP for not completing the exercise.",
            "Timeout",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Warning);
        
        Close();
    }

    private void ShowLevelUpNotification()
    {
        var levelUpWindow = new LevelUpWindow(_settings.Progress.Level);
        levelUpWindow.Show();
    }

    private void UpdateDeskPosition()
    {
        DeskPosition? newPosition = _cue.Type switch
        {
            CueType.DeskStanding => DeskPosition.Standing,
            CueType.DeskSitting => DeskPosition.Sitting,
            CueType.DeskFloor => DeskPosition.Floor,
            _ => null
        };

        if (newPosition.HasValue)
        {
            _scheduler.UpdatePosition(newPosition.Value);
        }
    }
}
