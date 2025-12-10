using System.Windows;
using System.Windows.Media.Animation;

namespace ErgoHealthCue.Windows;

public partial class BadgeUnlockedWindow : Window
{
    public BadgeUnlockedWindow(string badgeName, int streakRequired)
    {
        InitializeComponent();
        
        BadgeNameText.Text = badgeName;
        BadgeDescriptionText.Text = $"Complete {streakRequired} cues in a row!";
        
        // Start animations
        Loaded += (s, e) => StartAnimations();
        
        // Auto-close after 4 seconds
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            Close();
        };
        timer.Start();
    }
    
    private void StartAnimations()
    {
        // Scale up animation
        var scaleTransform = new System.Windows.Media.ScaleTransform(0.5, 0.5);
        ContentBorder.RenderTransform = scaleTransform;
        ContentBorder.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        
        var scaleAnimation = new DoubleAnimation
        {
            From = 0.5,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(0.3),
            EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
        };
        
        scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleAnimation);
        scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleAnimation);
        
        // Fade in animation
        var fadeAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.3)
        };
        BeginAnimation(OpacityProperty, fadeAnimation);
    }
}
