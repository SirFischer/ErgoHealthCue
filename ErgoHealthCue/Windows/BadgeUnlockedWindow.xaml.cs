using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ErgoHealthCue.Windows;

public partial class BadgeUnlockedWindow : Window
{
    public BadgeUnlockedWindow(string badgeName, int streakRequired, bool isNegative = false)
    {
        InitializeComponent();
        
        BadgeNameText.Text = badgeName;
        
        if (isNegative)
        {
            // Negative badge styling - humorous/sarcastic
            BadgeDescriptionText.Text = $"You dismissed {streakRequired} cues in a row. Really? 😏";
            BadgeIcon.Text = "💩"; // Poop emoji for negative badges
            TitleText.Text = "Achievement Unlocked?";
            TitleText.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // Red
            ContentBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // Red border
        }
        else
        {
            // Positive badge styling - celebratory
            BadgeDescriptionText.Text = $"Complete {streakRequired} cues in a row!";
            BadgeIcon.Text = "🏆"; // Trophy for positive badges
            TitleText.Text = "Badge Unlocked!";
            TitleText.Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Gold
            ContentBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Gold border
        }
        
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
        var scaleTransform = new ScaleTransform(0.5, 0.5);
        ContentBorder.RenderTransform = scaleTransform;
        ContentBorder.RenderTransformOrigin = new Point(0.5, 0.5);
        
        var scaleAnimation = new DoubleAnimation
        {
            From = 0.5,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(0.3),
            EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
        };
        
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        
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
