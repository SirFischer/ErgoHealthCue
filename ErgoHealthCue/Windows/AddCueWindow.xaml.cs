using System.Windows;
using ErgoHealthCue.Models;
using MessageBox = System.Windows.MessageBox;

namespace ErgoHealthCue.Windows;

public partial class AddCueWindow : Window
{
    public Cue NewCue { get; private set; } = new();

    public AddCueWindow(CueType? defaultType = null)
    {
        InitializeComponent();
        
        // Populate type combo box
        TypeComboBox.ItemsSource = Enum.GetValues(typeof(CueType));
        
        if (defaultType.HasValue)
        {
            TypeComboBox.SelectedItem = defaultType.Value;
        }
        else
        {
            TypeComboBox.SelectedIndex = 0;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Please enter a title.", "Missing Title", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            MessageBox.Show("Please enter a description.", "Missing Description", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        NewCue = new Cue
        {
            Type = (CueType)TypeComboBox.SelectedItem,
            Title = TitleTextBox.Text.Trim(),
            Description = DescriptionTextBox.Text.Trim(),
            IsEnabled = true
        };
        
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
