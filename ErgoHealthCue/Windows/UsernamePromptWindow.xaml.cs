using System.Windows;

namespace ErgoHealthCue.Windows;

public partial class UsernamePromptWindow : Window
{
    public string Username { get; private set; } = string.Empty;
    public bool LeaderboardEnabled { get; private set; } = true;
    
    public UsernamePromptWindow()
    {
        InitializeComponent();
        
        // Focus the username textbox
        Loaded += (s, e) => UsernameTextBox.Focus();
    }
    
    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        Username = UsernameTextBox.Text?.Trim() ?? string.Empty;
        LeaderboardEnabled = !(OptOutCheckBox.IsChecked ?? false);
        
        DialogResult = true;
        Close();
    }
}
