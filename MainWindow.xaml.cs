using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace WindowsMirror;

public partial class MainWindow : Window
{
    private readonly Settings _settings;

    public MainWindow()
    {
        _settings = Settings.Load();
        
        InitializeUI();
        ApplyWindowSettings();
        
        // Handle window closing
        Closing += MainWindow_Closing;
    }

    private void InitializeUI()
    {
        Title = "Windows Mirror";
        Width = 800;
        Height = 600;
        MinWidth = 400;
        MinHeight = 300;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // Video display area
        var videoBorder = new Border
        {
            Background = Brushes.Black,
            Margin = new Thickness(5)
        };
        Grid.SetRow(videoBorder, 0);

        var videoPanel = new Panel();
        var backgroundRect = new Rectangle { Fill = Brushes.DarkGray };
        videoPanel.Children.Add(backgroundRect);

        var statusText = new TextBlock
        {
            Name = "StatusText",
            Text = "Windows Mirror - Webcam functionality will be implemented soon.\nThis demonstrates the application structure and settings.",
            Foreground = Brushes.White,
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(20)
        };
        videoPanel.Children.Add(statusText);
        videoBorder.Child = videoPanel;

        // Control panel
        var controlPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(5)
        };
        Grid.SetRow(controlPanel, 1);

        var cameraSettingsButton = new Button
        {
            Content = "Camera Settings",
            Margin = new Thickness(5),
            Padding = new Thickness(10, 5)
        };
        cameraSettingsButton.Click += CameraSettingsButton_Click;

        var settingsButton = new Button
        {
            Content = "App Settings",
            Margin = new Thickness(5),
            Padding = new Thickness(10, 5)
        };
        settingsButton.Click += SettingsButton_Click;

        controlPanel.Children.Add(cameraSettingsButton);
        controlPanel.Children.Add(settingsButton);

        mainGrid.Children.Add(videoBorder);
        mainGrid.Children.Add(controlPanel);

        Content = mainGrid;
    }

    private void ApplyWindowSettings()
    {
        if (_settings.WindowWidth > 0 && _settings.WindowHeight > 0)
        {
            Width = _settings.WindowWidth;
            Height = _settings.WindowHeight;
        }
        
        if (_settings.WindowLeft >= 0 && _settings.WindowTop >= 0)
        {
            Position = new Avalonia.PixelPoint((int)_settings.WindowLeft, (int)_settings.WindowTop);
        }
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        // Save window settings
        _settings.WindowWidth = Width;
        _settings.WindowHeight = Height;
        _settings.WindowLeft = Position.X;
        _settings.WindowTop = Position.Y;
        _settings.Save();
    }

    private void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();
    }

    private void CameraSettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        ShowMessage("Camera Settings", 
            "Camera Settings\n\n" +
            "Camera settings functionality will open the platform-specific camera configuration dialog.\n\n" +
            "On Windows, this would access the camera properties through DirectShow interfaces.");
    }

    private void ShowMessage(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 400,
            Height = 250,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Children =
                {
                    new TextBlock 
                    { 
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 20)
                    },
                    new Button 
                    { 
                        Content = "OK", 
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Padding = new Thickness(20, 5)
                    }
                }
            }
        };
        
        var okButton = (Button)((StackPanel)messageBox.Content).Children[1];
        okButton.Click += (s, e) => messageBox.Close();
        
        messageBox.ShowDialog(this);
    }
}