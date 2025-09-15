# Windows Mirror Application Icon

This directory should contain the application icon files:

- mirror.ico (Windows icon)
- mirror.png (General use icon) 
- mirror.icns (macOS icon)

For the current implementation, the application uses the default system application icon.

To add custom icons:
1. Create appropriate icon files in multiple sizes (16x16, 32x32, 48x48, 64x64, 128x128, 256x256)
2. Add to the project file:
   ```xml
   <PropertyGroup>
     <ApplicationIcon>mirror.ico</ApplicationIcon>
   </PropertyGroup>
   ```
3. Update SystemTrayService to use custom icon:
   ```csharp
   _trayIcon.Icon = new WindowIcon("mirror.ico");
   ```

Recommended icon design:
- Simple mirror/reflection symbol
- High contrast for visibility in system tray
- Professional appearance suitable for desktop applications