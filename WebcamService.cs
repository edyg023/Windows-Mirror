using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace WindowsMirror;

public interface IWebcamService
{
    event Action<Bitmap>? FrameAvailable;
    bool IsAvailable { get; }
    Task StartAsync();
    Task StopAsync();
    void OpenSettings();
}

public class WebcamService : IWebcamService
{
    public event Action<Bitmap>? FrameAvailable;
    
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning;
    
    public bool IsAvailable => CheckWebcamAvailability();

    public async Task StartAsync()
    {
        if (_isRunning) return;
        
        _cancellationTokenSource = new CancellationTokenSource();
        _isRunning = true;
        
        // Start the simulation loop
        await SimulateWebcamFeedAsync(_cancellationTokenSource.Token);
    }

    public async Task StopAsync()
    {
        if (!_isRunning) return;
        
        _cancellationTokenSource?.Cancel();
        _isRunning = false;
        
        await Task.Delay(100); // Give time for cancellation
    }

    public void OpenSettings()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                // On Windows, this would open camera settings
                // For now, just show a placeholder message
                System.Diagnostics.Debug.WriteLine("Camera settings would open on Windows");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening camera settings: {ex.Message}");
            }
        }
    }

    private static bool CheckWebcamAvailability()
    {
        // In a real implementation, this would check for actual webcam devices
        // For now, we'll simulate that a webcam is available
        return true;
    }

    private async Task SimulateWebcamFeedAsync(CancellationToken cancellationToken)
    {
        // Create a simple animated pattern to simulate a webcam feed
        var random = new Random();
        int frameCount = 0;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Create a bitmap with a simple pattern
                var bitmap = CreateSimulatedFrame(frameCount, random);
                
                // Notify subscribers on the UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    FrameAvailable?.Invoke(bitmap);
                });
                
                frameCount++;
                
                // Simulate ~30 FPS
                await Task.Delay(33, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in webcam simulation: {ex.Message}");
                await Task.Delay(1000, cancellationToken); // Wait before retrying
            }
        }
    }

    private static Bitmap CreateSimulatedFrame(int frameCount, Random random)
    {
        const int width = 320;
        const int height = 240;
        
        // Create a simple bitmap with animated content
        var bitmap = new WriteableBitmap(
            new Avalonia.PixelSize(width, height),
            new Avalonia.Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888);

        using var lockedBitmap = bitmap.Lock();
        
        unsafe
        {
            var ptr = (uint*)lockedBitmap.Address;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Create an animated gradient pattern
                    var offset = (frameCount * 2) % 360;
                    var r = (byte)(Math.Sin((x + offset) * 0.02) * 127 + 128);
                    var g = (byte)(Math.Sin((y + offset) * 0.02) * 127 + 128);
                    var b = (byte)(Math.Sin((x + y + offset) * 0.01) * 127 + 128);
                    
                    // Add some "mirror-like" circular pattern
                    var centerX = width / 2;
                    var centerY = height / 2;
                    var distance = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                    var wave = Math.Sin(distance * 0.1 + frameCount * 0.1) * 50;
                    
                    r = (byte)Math.Max(0, Math.Min(255, r + wave));
                    g = (byte)Math.Max(0, Math.Min(255, g + wave));
                    b = (byte)Math.Max(0, Math.Min(255, b + wave));
                    
                    // BGRA format
                    ptr[y * width + x] = 0xFF000000 | ((uint)r << 16) | ((uint)g << 8) | b;
                }
            }
        }
        
        return bitmap;
    }
}

// Real webcam implementation would use platform-specific APIs
// For Windows: DirectShow, Media Foundation, or Windows.Media.Capture
// For Linux: V4L2 or GStreamer
// For macOS: AVFoundation