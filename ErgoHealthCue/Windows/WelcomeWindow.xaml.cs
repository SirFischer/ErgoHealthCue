using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ErgoHealthCue.Windows;

public partial class WelcomeWindow : Window
{
    public WelcomeWindow()
    {
        InitializeComponent();
        
        // Auto-close after 5 seconds
        var autoCloseTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        autoCloseTimer.Tick += (s, e) =>
        {
            autoCloseTimer.Stop();
            Close();
        };
        autoCloseTimer.Start();
        
        // Animate the content border
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
        var scaleIn = new DoubleAnimation(0.8, 1, TimeSpan.FromMilliseconds(500))
        {
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 }
        };
        
        var transform = new System.Windows.Media.ScaleTransform(0.8, 0.8);
        ContentBorder.RenderTransform = transform;
        ContentBorder.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
        
        this.BeginAnimation(OpacityProperty, fadeIn);
        transform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleIn);
        transform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleIn);
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
