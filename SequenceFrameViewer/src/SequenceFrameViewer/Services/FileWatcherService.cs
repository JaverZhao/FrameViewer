using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SequenceFrameViewer.Services;

public class FileWatcherService : IDisposable
{
    private FileSystemWatcher? _watcher;
    private Timer? _debounceTimer;
    private bool _disposed;
    private readonly int _debounceDelayMs = 500;

    public event Action? FilesChanged;

    public void StartWatch(string folderPath)
    {
        StopWatch();

        if (!Directory.Exists(folderPath))
            return;

        try
        {
            _watcher = new FileSystemWatcher(folderPath)
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite
            };

            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;
        }
        catch
        {
            // Watcher creation failed - non-critical
        }
    }

    public void StopWatch()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= OnChanged;
            _watcher.Deleted -= OnChanged;
            _watcher.Renamed -= OnChanged;
            _watcher.Changed -= OnChanged;
            _watcher.Dispose();
            _watcher = null;
        }

        _debounceTimer?.Dispose();
        _debounceTimer = null;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ =>
        {
            Task.Run(() => FilesChanged?.Invoke());
        }, null, _debounceDelayMs, Timeout.Infinite);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StopWatch();
    }
}
