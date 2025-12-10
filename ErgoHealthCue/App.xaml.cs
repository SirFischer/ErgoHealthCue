using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using ErgoHealthCue.Models;
using ErgoHealthCue.Services;
using ErgoHealthCue.Windows;
using Application = System.Windows.Application;
using Strings = ErgoHealthCue.Resources.Strings;

namespace ErgoHealthCue;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    private NotifyIcon? _notifyIcon;
    private DataService? _dataService;
    private CueScheduler? _scheduler;
    private AppSettings? _settings;
    private StartupService? _startupService;
    private ToolStripMenuItem? _pauseResumeMenuItem;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Initialize services
        _dataService = new DataService();
        _startupService = new StartupService();
        _settings = _dataService.LoadSettings();
        
        // Set language/culture
        SetApplicationLanguage(_settings.Language);
        _scheduler = new CueScheduler(_settings, _dataService);
        
        // Subscribe to cue events
        _scheduler.CueTriggered += Scheduler_CueTriggered;
        _scheduler.CueTriggeredManually += Scheduler_CueTriggeredManually;
        _scheduler.PauseEnded += Scheduler_PauseEnded;
        
        // Start the scheduler
        _scheduler.Start();
        
        // Create system tray icon
        SetupNotifyIcon();
        
        // Don't show main window on startup
        MainWindow = new MainWindow();
    }
    
    private void Scheduler_PauseEnded(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            if (_pauseResumeMenuItem != null)
            {
                _pauseResumeMenuItem.Text = Strings.PauseCues;
            }
            _notifyIcon!.Text = Strings.AppTitle;
        });
    }

    private void SetupNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon(GetIconStream()),
            Visible = true,
            Text = Strings.AppTitle
        };

        var contextMenu = new ContextMenuStrip();
        
        // Pause/Resume menu item (shown first for easy access)
        _pauseResumeMenuItem = new ToolStripMenuItem(Strings.PauseCues, null, (s, e) => TogglePauseResume());
        _pauseResumeMenuItem.Font = new System.Drawing.Font(_pauseResumeMenuItem.Font, System.Drawing.FontStyle.Bold);
        contextMenu.Items.Add(_pauseResumeMenuItem);
        contextMenu.Items.Add("-");
        
        contextMenu.Items.Add(Strings.TriggerCueNow, null, (s, e) => TriggerCueNow());
        contextMenu.Items.Add("-");
        contextMenu.Items.Add(Strings.Settings, null, (s, e) => OpenSettings());
        contextMenu.Items.Add(Strings.ProgressStatistics, null, (s, e) => OpenStatistics());
        contextMenu.Items.Add("-");
        contextMenu.Items.Add(Strings.Exit, null, (s, e) => Shutdown());

        _notifyIcon.ContextMenuStrip = contextMenu;
        _notifyIcon.DoubleClick += (s, e) => OpenSettings();
        _notifyIcon.Click += (s, e) => 
        {
            var mouseEvent = e as System.Windows.Forms.MouseEventArgs;
            if (mouseEvent?.Button == MouseButtons.Left)
            {
                TogglePauseResume();
            }
        };
    }

    private System.IO.Stream GetIconStream()
    {
        // Create a simple icon programmatically
        using var bitmap = new System.Drawing.Bitmap(32, 32);
        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            g.Clear(System.Drawing.Color.Transparent);
            g.FillEllipse(System.Drawing.Brushes.Blue, 4, 4, 24, 24);
            using var font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            g.DrawString("E", font, System.Drawing.Brushes.White, 8, 6);
        }
        
        var iconHandle = bitmap.GetHicon();
        try
        {
            using var icon = System.Drawing.Icon.FromHandle(iconHandle);
            var stream = new System.IO.MemoryStream();
            icon.Save(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return stream;
        }
        finally
        {
            // Clean up the GDI handle to prevent leaks
            DestroyIcon(iconHandle);
        }
    }

    private void Scheduler_CueTriggered(object? sender, Cue cue)
    {
        Dispatcher.Invoke(() =>
        {
            var overlayWindow = new CueOverlayWindow(cue, _dataService!, _scheduler!, _settings!, false);
            overlayWindow.Show();
        });
    }

    private void Scheduler_CueTriggeredManually(object? sender, (Cue cue, bool isManual) args)
    {
        Dispatcher.Invoke(() =>
        {
            var overlayWindow = new CueOverlayWindow(args.cue, _dataService!, _scheduler!, _settings!, args.isManual);
            overlayWindow.Show();
        });
    }

    private void TriggerCueNow()
    {
        Dispatcher.Invoke(() =>
        {
            _scheduler?.TriggerNow(true);
        });
    }

    private void TogglePauseResume()
    {
        Dispatcher.Invoke(() =>
        {
            if (_scheduler?.IsPaused == true)
            {
                _scheduler.Resume();
                if (_pauseResumeMenuItem != null)
                {
                    _pauseResumeMenuItem.Text = Strings.PauseCues;
                }
                _notifyIcon!.Text = Strings.AppTitle;
            }
            else
            {
                // Show pause duration dialog
                var pauseDialog = new PauseDurationWindow();
                if (pauseDialog.ShowDialog() == true)
                {
                    _scheduler?.Pause(pauseDialog.PauseDuration);
                    if (_pauseResumeMenuItem != null)
                    {
                        _pauseResumeMenuItem.Text = Strings.ResumeCues;
                    }
                    _notifyIcon!.Text = Strings.AppTitle + " (Paused)";
                }
            }
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

    private void SetApplicationLanguage(string language)
    {
        var culture = language switch
        {
            "no" => new System.Globalization.CultureInfo("no"),
            "en" => new System.Globalization.CultureInfo("en"),
            "fr" => new System.Globalization.CultureInfo("fr"),
            "auto" or _ => System.Globalization.CultureInfo.CurrentUICulture
        };
        
        System.Globalization.CultureInfo.CurrentUICulture = culture;
        System.Globalization.CultureInfo.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        _scheduler?.Stop();
    }
}


