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
    private readonly CueStatistic _statistic;
    private readonly DispatcherTimer _flashTimer;
    private bool _isFlashOn = true;

    public CueOverlayWindow(Cue cue, DataService dataService)
    {
        InitializeComponent();
        _cue = cue;
        _dataService = dataService;

        TitleText.Text = cue.Title;
        DescriptionText.Text = cue.Description;

        _statistic = new CueStatistic
        {
            CueId = cue.Id,
            CueTitle = cue.Title,
            CueType = cue.Type,
            ShownAt = DateTime.Now
        };

        // Setup flashing effect
        _flashTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _flashTimer.Tick += FlashTimer_Tick;
        _flashTimer.Start();

        // Play a subtle animation on show
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        BeginAnimation(OpacityProperty, fadeIn);
    }

    private void FlashTimer_Tick(object? sender, EventArgs e)
    {
        _isFlashOn = !_isFlashOn;
        MainBorder.BorderBrush = new SolidColorBrush(_isFlashOn ? 
            System.Windows.Media.Color.FromRgb(33, 150, 243) : 
            System.Windows.Media.Color.FromRgb(255, 152, 0));
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        _flashTimer.Stop();
        _statistic.CompletedAt = DateTime.Now;
        _statistic.WasCompleted = true;
        _dataService.AddStatistic(_statistic);
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
}
