# Windows Mirror

WindowsMirror is a modern, cross-platform application that displays a live video feed from your webcam, acting as a digital mirror. The application is built using Avalonia UI for modern theming and cross-platform support.

## Features

### ✅ Implemented
- **Modern UI with Windows Theme Support**: Built with Avalonia UI and Fluent theme
- **Window Management**: 
  - Configurable default window size
  - Automatic save/restore of window position and size on exit
  - Resizable window with minimum size constraints
- **Settings Persistence**: JSON-based configuration storage
- **Windows Startup Integration**: Option to start with Windows (Windows only)
- **System Tray Support**: Minimize to system tray (Windows/Linux)
- **Cross-Platform**: Runs on Windows, Linux, and macOS
- **Webcam Simulation**: Demonstrates video display with animated content

### 🔧 Architecture Ready
- **Webcam Service Interface**: Extensible design for real webcam integration
- **Platform-Specific Camera Settings**: Framework for native camera configuration
- **Taskbar Integration**: Built-in support through Avalonia framework

## Requirements

- .NET 8.0 or later
- Windows 10/11, Linux (X11), or macOS for GUI support

## Building and Running

### Prerequisites
```bash
# Install .NET 8 SDK
# Windows: Download from https://dotnet.microsoft.com/download
# Linux: sudo apt install dotnet-sdk-8.0
# macOS: brew install dotnet
```

### Build
```bash
git clone https://github.com/edyg023/Windows-Mirror.git
cd Windows-Mirror
dotnet restore
dotnet build
```

### Run
```bash
dotnet run
```

## Application Structure

### Core Components

1. **MainWindow**: Primary application window with video display
2. **SettingsWindow**: Configuration dialog for user preferences
3. **Settings**: Persistent configuration management
4. **WebcamService**: Abstracted webcam interface (currently simulated)
5. **SystemTrayService**: Cross-platform system tray integration

### File Structure
```
WindowsMirror/
├── App.xaml                 # Application resources and theme
├── App.xaml.cs              # Application lifecycle and tray management
├── MainWindow.xaml.cs       # Main window with video display
├── SettingsWindow.xaml.cs   # Settings configuration dialog
├── Settings.cs              # Configuration persistence
├── WebcamService.cs         # Webcam abstraction layer
├── SystemTrayService.cs     # System tray integration
├── Program.cs               # Application entry point
├── WindowsMirror.csproj     # Project configuration
└── app.manifest             # Windows application manifest
```

## Configuration

Settings are automatically saved to:
- **Windows**: `%APPDATA%\WindowsMirror\settings.json`
- **Linux**: `~/.config/WindowsMirror/settings.json`
- **macOS**: `~/Library/Application Support/WindowsMirror/settings.json`

### Available Settings
- `WindowWidth`/`WindowHeight`: Current window dimensions
- `WindowLeft`/`WindowTop`: Window position
- `DefaultWidth`/`DefaultHeight`: Default window size for new instances
- `StartWithWindows`: Auto-start with system boot (Windows only)

## Features in Detail

### Modern UI and Theming
The application uses Avalonia UI with the Fluent theme, providing:
- Native look and feel on each platform
- Automatic light/dark theme following system preferences
- Smooth animations and modern controls
- High DPI support

### Webcam Integration (Framework)
Current implementation includes:
- **Simulated Feed**: Animated gradient pattern for demonstration
- **Extensible Interface**: `IWebcamService` for real camera implementation
- **Error Handling**: Graceful fallback when cameras are unavailable
- **Platform Abstraction**: Ready for Windows (DirectShow/WinRT), Linux (V4L2), macOS (AVFoundation)

### System Tray Integration
- **Minimize to Tray**: Window hides to system tray instead of closing
- **Context Menu**: Right-click menu with Show/Exit options
- **Double-Click Restore**: Double-click tray icon to restore window
- **Cross-Platform**: Works on Windows and Linux (requires system tray support)

### Windows Integration
- **Registry Management**: Automatic Windows startup configuration
- **Application Manifest**: Proper Windows application metadata
- **DPI Awareness**: High DPI display support

## Future Enhancements

### Real Webcam Implementation
```csharp
// Windows implementation example
public class WindowsWebcamService : IWebcamService
{
    // DirectShow or Windows.Media.Capture implementation
}

// Linux implementation example  
public class LinuxWebcamService : IWebcamService
{
    // V4L2 or GStreamer implementation
}
```

### Advanced Features
- **Multiple Camera Support**: Camera selection dropdown
- **Video Effects**: Filters, overlays, and transformations
- **Recording**: Save video clips or screenshots
- **Zoom and Pan**: Digital zoom and positioning controls
- **Fullscreen Mode**: Dedicated fullscreen mirror view

## Technical Details

### Dependencies
- **Avalonia UI 11.0.7**: Cross-platform UI framework
- **System.Drawing.Common**: Image processing support
- **Microsoft.Win32.Registry**: Windows registry access

### Platform Compatibility
- **Windows 10/11**: Full feature support including startup integration
- **Linux (X11)**: Core functionality with system tray support
- **macOS**: Core functionality (system tray may have limitations)

### Performance
- **30 FPS Target**: Smooth video display
- **Efficient Memory Usage**: Proper bitmap disposal and memory management
- **Responsive UI**: Asynchronous video processing

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Support

For issues and feature requests, please use the GitHub issue tracker.
