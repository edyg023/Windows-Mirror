using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace WindowsMirror;

public partial class MainWindow : Window
{
    private readonly Settings _settings;
    private readonly IWebcamService _webcamService;
    private Image? _videoImage;
    private TextBlock? _statusText;

    public MainWindow()
    {
        _settings = Settings.Load();
        _webcamService = new WebcamService();
        
        InitializeUI();
        ApplyWindowSettings();
        
        // Handle window closing
        Closing += MainWindow_Closing;
        
        // Subscribe to webcam events
        _webcamService.FrameAvailable += OnFrameAvailable;
        
        // Start webcam when window loads
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        if (_webcamService.IsAvailable)
        {
            if (_statusText != null)
            {
                _statusText.Text = "Starting webcam...";
            }
            
            try
            {
                await _webcamService.StartAsync();
                
                if (_statusText != null)
                {
                    _statusText.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                if (_statusText != null)
                {
                    _statusText.Text = $"Error starting webcam: {ex.Message}";
                }
            }
        }
        else
        {
            if (_statusText != null)
            {
                _statusText.Text = "No webcam detected.\nThis is a demonstration with simulated video feed.";
            }
        }
    }

    private void OnFrameAvailable(Bitmap frame)
    {
        if (_videoImage != null)
        {
            _videoImage.Source = frame;
        }
        
        if (_statusText != null && _statusText.IsVisible)
        {
            _statusText.IsVisible = false;
        }
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
        
        // Video image
        _videoImage = new Image
        {
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.Both
        };
        videoPanel.Children.Add(_videoImage);

        // Status text overlay
        _statusText = new TextBlock
        {
            Text = "Initializing Windows Mirror...",
            Foreground = Brushes.White,
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(20),
            Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)) // Semi-transparent background
        };
        videoPanel.Children.Add(_statusText);
        
        videoBorder.Child = videoPanel;

        // Control panel
        var controlPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(5),
            Background = new SolidColorBrush(Color.FromArgb(240, 240, 240, 240)) // Light semi-transparent background
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

    private async void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        // Save window settings
        _settings.WindowWidth = Width;
        _settings.WindowHeight = Height;
        _settings.WindowLeft = Position.X;
        _settings.WindowTop = Position.Y;
        _settings.Save();
        
        // Stop webcam
        await _webcamService.StopAsync();
    }

    private void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();
    }

    private void CameraSettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        _webcamService.OpenSettings();
        
        ShowMessage("Camera Settings", 
            "Camera Settings\n\n" +
            "In a full implementation, this would open platform-specific camera settings:\n\n" +
            "• Windows: DirectShow property pages\n" +
            "• Linux: V4L2 control interface\n" +
            "• macOS: AVFoundation settings\n\n" +
            "Current implementation shows a simulated webcam feed for demonstration.");
    }

    private void ShowMessage(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 450,
            Height = 300,
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