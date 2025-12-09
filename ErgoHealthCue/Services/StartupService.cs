using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace ErgoHealthCue.Services;

public class StartupService
{
    private const string AppName = "ErgoHealthCue";
    private readonly string _runKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public bool IsStartupEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(_runKeyPath, false);
            var value = key?.GetValue(AppName);
            return value != null;
        }
        catch
        {
            return false;
        }
    }

    public void SetStartup(bool enable)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(_runKeyPath, true);
            
            if (enable)
            {
                // Use Process.GetCurrentProcess().MainModule.FileName for more reliable path
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    key?.SetValue(AppName, $"\"{exePath}\"");
                }
            }
            else
            {
                key?.DeleteValue(AppName, false);
            }
        }
        catch (Exception ex)
        {
            // Log or handle error
            System.Diagnostics.Debug.WriteLine($"Failed to set startup: {ex.Message}");
        }
    }
}
