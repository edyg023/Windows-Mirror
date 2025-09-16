using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace WindowsMirror;

public interface ISystemTrayService
{
    bool IsSupported { get; }
    void Initialize();
    void Show();
    void Hide();
    void SetTooltip(string tooltip);
    event Action? ShowWindowRequested;
    event Action? ExitRequested;
    void Dispose();
}

public class SystemTrayService : ISystemTrayService, IDisposable
{
    private TrayIcon? _trayIcon;
    private bool _isInitialized;

    public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                              RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public event Action? ShowWindowRequested;
    public event Action? ExitRequested;

    public void Initialize()
    {
        if (_isInitialized || !IsSupported) return;

        try
        {
            _trayIcon = new TrayIcon
            {
                ToolTipText = "Windows Mirror",
                IsVisible = true
            };

            // Create context menu
            var menu = new NativeMenu();
            
            var showItem = new NativeMenuItem("Show Window");
            showItem.Click += (s, e) => ShowWindowRequested?.Invoke();
            
            var exitItem = new NativeMenuItem("Exit");
            exitItem.Click += (s, e) => ExitRequested?.Invoke();
            
            menu.Add(showItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(exitItem);
            
            _trayIcon.Menu = menu;
            
            // Handle double-click
            _trayIcon.Clicked += (s, e) => ShowWindowRequested?.Invoke();

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize system tray: {ex.Message}");
        }
    }

    public void Show()
    {
        if (_trayIcon != null)
        {
            _trayIcon.IsVisible = true;
        }
    }

    public void Hide()
    {
        if (_trayIcon != null)
        {
            _trayIcon.IsVisible = false;
        }
    }

    public void SetTooltip(string tooltip)
    {
        if (_trayIcon != null)
        {
            _trayIcon.ToolTipText = tooltip;
        }
    }

    public void Dispose()
    {
        _trayIcon?.Dispose();
        _trayIcon = null;
        _isInitialized = false;
    }
}