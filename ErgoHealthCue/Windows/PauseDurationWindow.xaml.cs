using System.Windows;

namespace ErgoHealthCue.Windows;

public partial class PauseDurationWindow : Window
{
    public TimeSpan PauseDuration { get; private set; }
    
    public PauseDurationWindow()
    {
        InitializeComponent();
    }
    
    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate and parse input
        if (!int.TryParse(HoursTextBox.Text, out int hours) || hours < 0)
        {
            ErrorText.Text = "Please enter a valid number of hours (0-8)";
            return;
        }
        
        if (!int.TryParse(MinutesTextBox.Text, out int minutes) || minutes < 0 || minutes >= 60)
        {
            ErrorText.Text = "Please enter valid minutes (0-59)";
            return;
        }
        
        // Check total duration
        var totalMinutes = hours * 60 + minutes;
        if (totalMinutes == 0)
        {
            ErrorText.Text = "Please enter a duration greater than 0";
            return;
        }
        
        if (totalMinutes > 480) // 8 hours max
        {
            ErrorText.Text = "Maximum pause duration is 8 hours";
            return;
        }
        
        PauseDuration = TimeSpan.FromMinutes(totalMinutes);
        DialogResult = true;
        Close();
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
