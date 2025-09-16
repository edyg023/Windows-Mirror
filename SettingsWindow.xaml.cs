using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace WindowsMirror;

public partial class SettingsWindow : Window
{
    private readonly Settings _settings;
    private TextBox? _widthTextBox;
    private TextBox? _heightTextBox;
    private CheckBox? _startupCheckBox;

    public SettingsWindow()
    {
        _settings = Settings.Load();
        InitializeUI();
        LoadSettings();
    }

    private void InitializeUI()
    {
        Title = "Settings - Windows Mirror";
        Width = 400;
        Height = 350;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;

        var mainGrid = new Grid { Margin = new Thickness(20) };
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // Title
        var titleText = new TextBlock
        {
            Text = "Windows Mirror Settings",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 20)
        };
        Grid.SetRow(titleText, 0);

        // Settings panel
        var settingsPanel = new StackPanel();
        Grid.SetRow(settingsPanel, 1);

        // Window size group
        var sizeGroup = CreateGroupBox("Default Window Size");
        var sizeGrid = new Grid { Margin = new Thickness(10) };
        sizeGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        sizeGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        sizeGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(20)));
        sizeGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        sizeGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var widthLabel = new TextBlock { Text = "Width:", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(widthLabel, 0);

        _widthTextBox = new TextBox { Text = "800", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0) };
        Grid.SetColumn(_widthTextBox, 1);

        var heightLabel = new TextBlock { Text = "Height:", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(heightLabel, 3);

        _heightTextBox = new TextBox { Text = "600", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0) };
        Grid.SetColumn(_heightTextBox, 4);

        sizeGrid.Children.Add(widthLabel);
        sizeGrid.Children.Add(_widthTextBox);
        sizeGrid.Children.Add(heightLabel);
        sizeGrid.Children.Add(_heightTextBox);

        sizeGroup.Child = sizeGrid;
        settingsPanel.Children.Add(sizeGroup);

        // Startup group
        var startupGroup = CreateGroupBox("Startup");
        var startupStack = new StackPanel { Margin = new Thickness(10) };
        
        _startupCheckBox = new CheckBox 
        { 
            Content = "Start with Windows", 
            Margin = new Thickness(0, 5) 
        };
        
        var startupNote = new TextBlock
        {
            Text = "When enabled, Windows Mirror will start automatically when Windows starts.",
            FontSize = 10,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 5, 0, 0),
            TextWrapping = TextWrapping.Wrap
        };

        startupStack.Children.Add(_startupCheckBox);
        startupStack.Children.Add(startupNote);
        startupGroup.Child = startupStack;
        settingsPanel.Children.Add(startupGroup);

        // Buttons
        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 15, 0, 0)
        };
        Grid.SetRow(buttonPanel, 2);

        var resetButton = new Button
        {
            Content = "Reset Defaults",
            Margin = new Thickness(5),
            Padding = new Thickness(10, 5)
        };
        resetButton.Click += ResetDefaultsButton_Click;

        var cancelButton = new Button
        {
            Content = "Cancel",
            Margin = new Thickness(5),
            Padding = new Thickness(10, 5)
        };
        cancelButton.Click += CancelButton_Click;

        var saveButton = new Button
        {
            Content = "Save",
            Margin = new Thickness(5),
            Padding = new Thickness(10, 5)
        };
        saveButton.Click += SaveButton_Click;

        buttonPanel.Children.Add(resetButton);
        buttonPanel.Children.Add(cancelButton);
        buttonPanel.Children.Add(saveButton);

        mainGrid.Children.Add(titleText);
        mainGrid.Children.Add(settingsPanel);
        mainGrid.Children.Add(buttonPanel);

        Content = mainGrid;
    }

    private Border CreateGroupBox(string title)
    {
        return new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
            Margin = new Thickness(0, 0, 0, 15),
            Child = new StackPanel
            {
                Children =
                {
                    new TextBlock 
                    { 
                        Text = title, 
                        FontWeight = FontWeight.Bold, 
                        Margin = new Thickness(10, 5, 10, 0),
                        Background = Brushes.White
                    }
                }
            }
        };
    }

    private void LoadSettings()
    {
        if (_widthTextBox != null)
            _widthTextBox.Text = _settings.DefaultWidth.ToString();
        
        if (_heightTextBox != null)
            _heightTextBox.Text = _settings.DefaultHeight.ToString();
        
        if (_startupCheckBox != null)
            _startupCheckBox.IsChecked = _settings.IsStartupWithWindowsEnabled();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Parse and save default dimensions
            if (_widthTextBox != null &&
                int.TryParse(_widthTextBox.Text, out int width) && width > 0)
            {
                _settings.DefaultWidth = width;
            }
            
            if (_heightTextBox != null &&
                int.TryParse(_heightTextBox.Text, out int height) && height > 0)
            {
                _settings.DefaultHeight = height;
            }

            // Save startup setting
            if (_startupCheckBox != null)
            {
                _settings.SetStartupWithWindows(_startupCheckBox.IsChecked == true);
            }

            _settings.Save();
            
            ShowMessage("Settings", "Settings saved successfully!");
            Close();
        }
        catch (Exception ex)
        {
            ShowMessage("Error", $"Error saving settings: {ex.Message}");
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ResetDefaultsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_widthTextBox != null)
            _widthTextBox.Text = "800";
        
        if (_heightTextBox != null)
            _heightTextBox.Text = "600";
        
        if (_startupCheckBox != null)
            _startupCheckBox.IsChecked = false;
    }

    private void ShowMessage(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Children =
                {
                    new TextBlock 
                    { 
                        Text = message, 
                        Margin = new Thickness(0, 0, 0, 20),
                        TextWrapping = TextWrapping.Wrap
                    },
                    new Button 
                    { 
                        Content = "OK", 
                        HorizontalAlignment = HorizontalAlignment.Center 
                    }
                }
            }
        };
        
        var okButton = (Button)((StackPanel)messageBox.Content).Children[1];
        okButton.Click += (s, e) => messageBox.Close();
        
        messageBox.ShowDialog(this);
    }
}