using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;

namespace ErgoHealthCue.Windows;

public partial class CueOverlayWindow : Window
{
    private readonly Cue _cue;
    private readonly DataService _dataService;
    private readonly CueScheduler _scheduler;
    private readonly CueStatistic _statistic;
    private readonly DispatcherTimer _flashTimer;
    private bool _isFlashOn = true;

    public CueOverlayWindow(Cue cue, DataService dataService, CueScheduler scheduler)
    {
        InitializeComponent();
        _cue = cue;
        _dataService = dataService;
        _scheduler = scheduler;

        TitleText.Text = cue.Title;
        DescriptionText.Text = cue.Description;

        _statistic = new CueStatistic
        {
            CueId = cue.Id,
            CueTitle = cue.Title,
            CueType = cue.Type,
            ShownAt = DateTime.Now
        };

        // Setup flashing effect with modern colors
        _flashTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(600)
        };
        _flashTimer.Tick += FlashTimer_Tick;
        _flashTimer.Start();

        // Play a smooth animation on show
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

    private void FlashTimer_Tick(object? sender, EventArgs e)
    {
        _isFlashOn = !_isFlashOn;
        
        // Modern indigo gradient with pulsing effect
        var color1 = System.Windows.Media.Color.FromRgb(99, 102, 241);   // Indigo-500
        var color2 = System.Windows.Media.Color.FromRgb(139, 92, 246);   // Violet-500
        
        MainBorder.BorderBrush = new SolidColorBrush(_isFlashOn ? color1 : color2);
        IconCircle.Fill = new SolidColorBrush(_isFlashOn ? color1 : color2);
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        _flashTimer.Stop();
        _statistic.CompletedAt = DateTime.Now;
        _statistic.WasCompleted = true;
        _dataService.AddStatistic(_statistic);
        
        // Update desk position if this was a position change cue
        UpdateDeskPosition();
        
        Close();
    }

    private void DismissButton_Click(object sender, RoutedEventArgs e)
    {
        _flashTimer.Stop();
        _statistic.DismissedAt = DateTime.Now;
        _statistic.WasCompleted = false;
        _dataService.AddStatistic(_statistic);
        Close();
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
