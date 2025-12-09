using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;
using ErgoHealthCue.Windows;
using Application = System.Windows.Application;

namespace ErgoHealthCue;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NotifyIcon? _notifyIcon;
    private DataService? _dataService;
    private CueScheduler? _scheduler;
    private AppSettings? _settings;
    private StartupService? _startupService;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Initialize services
        _dataService = new DataService();
        _startupService = new StartupService();
        _settings = _dataService.LoadSettings();
        _scheduler = new CueScheduler(_settings, _dataService);
        
        // Subscribe to cue events
        _scheduler.CueTriggered += Scheduler_CueTriggered;
        
        // Start the scheduler
        _scheduler.Start();
        
        // Create system tray icon
        SetupNotifyIcon();
        
        // Don't show main window on startup
        MainWindow = new MainWindow();
    }

    private void SetupNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon(GetIconStream()),
            Visible = true,
            Text = "ErgoHealthCue"
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Settings", null, (s, e) => OpenSettings());
        contextMenu.Items.Add("Statistics", null, (s, e) => OpenStatistics());
        contextMenu.Items.Add("-");
        contextMenu.Items.Add("Exit", null, (s, e) => Shutdown());

        _notifyIcon.ContextMenuStrip = contextMenu;
        _notifyIcon.DoubleClick += (s, e) => OpenSettings();
    }

    private System.IO.Stream GetIconStream()
    {
        // Create a simple icon programmatically
        var bitmap = new System.Drawing.Bitmap(32, 32);
        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            g.Clear(System.Drawing.Color.Transparent);
            g.FillEllipse(System.Drawing.Brushes.Blue, 4, 4, 24, 24);
            g.DrawString("E", new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold), 
                System.Drawing.Brushes.White, 8, 6);
        }
        
        var iconHandle = bitmap.GetHicon();
        var icon = System.Drawing.Icon.FromHandle(iconHandle);
        var stream = new System.IO.MemoryStream();
        icon.Save(stream);
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        return stream;
    }

    private void Scheduler_CueTriggered(object? sender, Cue cue)
    {
        Dispatcher.Invoke(() =>
        {
            var overlayWindow = new CueOverlayWindow(cue, _dataService!);
            overlayWindow.Show();
        });
    }

    private void OpenSettings()
    {
        Dispatcher.Invoke(() =>
        {
            var settingsWindow = new SettingsWindow(_settings!, _dataService!, _startupService!);
            if (settingsWindow.ShowDialog() == true)
            {
                // Reload settings and update scheduler
                _settings = _dataService!.LoadSettings();
                _scheduler!.UpdateSettings(_settings);
            }
        });
    }

    private void OpenStatistics()
    {
        Dispatcher.Invoke(() =>
        {
            var statsWindow = new StatisticsWindow(_dataService!);
            statsWindow.ShowDialog();
        });
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        _scheduler?.Stop();
    }
}


