using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

namespace WindowsMirror;

public partial class App : Application
{
    private SystemTrayService? _trayService;
    private MainWindow? _mainWindow;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Initialize system tray
            _trayService = new SystemTrayService();
            
            if (_trayService.IsSupported)
            {
                _trayService.Initialize();
                _trayService.ShowWindowRequested += ShowMainWindow;
                _trayService.ExitRequested += ExitApplication;
            }

            // Create main window but don't show it immediately if tray is supported
            _mainWindow = new MainWindow();
            
            if (_trayService?.IsSupported == true)
            {
                // Start in tray if supported
                _mainWindow.WindowState = Avalonia.Controls.WindowState.Minimized;
                desktop.MainWindow = _mainWindow;
                
                // Override the closing behavior to minimize to tray
                _mainWindow.Closing += MainWindow_Closing;
            }
            else
            {
                // Show normally if tray is not supported
                desktop.MainWindow = _mainWindow;
                _mainWindow.Show();
            }
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_trayService?.IsSupported == true)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            if (_mainWindow != null)
            {
                _mainWindow.Hide();
            }
        }
    }

    private void ShowMainWindow()
    {
        if (_mainWindow != null)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = Avalonia.Controls.WindowState.Normal;
            _mainWindow.Activate();
        }
    }

    private void ExitApplication()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Clean shutdown
            _trayService?.Dispose();
            desktop.Shutdown();
        }
    }
}