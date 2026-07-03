using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SequenceFrameViewer.Models;
using SequenceFrameViewer.Services;
using SequenceFrameViewer.Views;

namespace SequenceFrameViewer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly SequenceScanner _scanner;
    private readonly FrameDecoder _decoder;
    private readonly PlaybackEngine _engine;
    private readonly FrameCache _cache;
    private readonly FileWatcherService _fileWatcher;

    private FrameSequence? _currentSequence;
    private bool _isPreloading;
    private bool _isUpdatingSlider;

    [ObservableProperty]
    private int _currentFrameIndex;

    [ObservableProperty]
    private int _totalFrames;

    [ObservableProperty]
    private string _currentFileName = string.Empty;

    [ObservableProperty]
    private string _frameSizeText = string.Empty;

    [ObservableProperty]
    private double _fps = 24;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isLooping = true;

    [ObservableProperty]
    private bool _isEmpty = true;

    [ObservableProperty]
    private string _emptyStateMessage = "拖入序列帧文件夹开始预览";

    [ObservableProperty]
    private string _backgroundMode = "Checkerboard";

    [ObservableProperty]
    private BitmapSource? _currentImage;

    [ObservableProperty]
    private double _zoomLevel = 1.0;

    [ObservableProperty]
    private bool _isFitToWindow = true;

    [ObservableProperty]
    private string _zoomText = "适配窗口";

    [ObservableProperty]
    private double _frameSliderValue;

    [ObservableProperty]
    private ObservableCollection<string> _recentFolders = new();

    public MainViewModel()
    {
        _settingsService = new SettingsService();
        _scanner = new SequenceScanner();
        _cache = new FrameCache(maxItems: 60);
        _decoder = new FrameDecoder(_cache);
        _engine = new PlaybackEngine();
        _fileWatcher = new FileWatcherService();

        _engine.FrameChanged += OnEngineFrameChanged;
        _engine.PlaybackEnded += OnEnginePlaybackEnded;
        _fileWatcher.FilesChanged += OnFolderChanged;

        LogService.Info("Application started");
        LoadSettings();
    }

    public void LoadSettings()
    {
        var settings = _settingsService.Load();
        Fps = settings.DefaultFps;
        IsLooping = settings.LoopPlayback;
        BackgroundMode = settings.BackgroundMode;
        _engine.Fps = settings.DefaultFps;
        _engine.Loop = settings.LoopPlayback;

        RecentFolders.Clear();
        foreach (var folder in settings.RecentFolders.Where(Directory.Exists))
        {
            RecentFolders.Add(folder);
        }
    }

    public void SaveSettings()
    {
        var settings = new AppSettings
        {
            DefaultFps = (int)Fps,
            LoopPlayback = IsLooping,
            BackgroundMode = BackgroundMode,
            RecentFolders = RecentFolders.ToList(),
            MaxCacheMemoryMb = 512,
            LastWindowWidth = Width,
            LastWindowHeight = Height
        };
        _settingsService.Save(settings);
    }

    public double Width { get; set; } = 1200;
    public double Height { get; set; } = 800;

    public async Task LoadFolder(string folderPath)
    {
        _engine.Stop();
        _fileWatcher.StopWatch();
        _cache.Clear();
        IsPlaying = false;
        IsEmpty = true;
        CurrentImage = null;
        CurrentFrameIndex = 0;
        TotalFrames = 0;
        CurrentFileName = string.Empty;
        FrameSizeText = string.Empty;
        FrameSliderValue = 0;
        FitToWindow();
        _currentSequence = null;

        var sequence = await Task.Run(() => _scanner.Scan(folderPath));

        if (sequence.IsEmpty)
        {
            EmptyStateMessage = "文件夹中没有支持的图片文件";
            LogService.Warning($"No supported images found in {folderPath}");
            return;
        }

        LogService.Info($"Loaded {sequence.TotalFrames} frames from {folderPath}");
        _currentSequence = sequence;
        _engine.LoadSequence(sequence);
        TotalFrames = sequence.TotalFrames;
        IsEmpty = false;
        EmptyStateMessage = string.Empty;

        AddRecentFolder(folderPath);
        _fileWatcher.StartWatch(folderPath);

        await GoToFrame(0);
    }

    private void AddRecentFolder(string folderPath)
    {
        RecentFolders.Remove(folderPath);
        RecentFolders.Insert(0, folderPath);
        while (RecentFolders.Count > 10)
            RecentFolders.RemoveAt(RecentFolders.Count - 1);
    }

    public async Task GoToFrame(int index)
    {
        if (_currentSequence == null || index < 0 || index >= _currentSequence.TotalFrames)
            return;

        var frame = _currentSequence.Frames[index];
        CurrentFrameIndex = index;
        CurrentFileName = frame.FileName;

        if (!_isUpdatingSlider)
            FrameSliderValue = index;

        var bitmap = await _decoder.DecodeAsync(frame.FilePath);
        CurrentImage = bitmap;

        if (bitmap != null)
        {
            frame.Width = bitmap.PixelWidth;
            frame.Height = bitmap.PixelHeight;
            FrameSizeText = $"{bitmap.PixelWidth} × {bitmap.PixelHeight}";
        }

        if (_engine.State == PlaybackState.Playing)
            _ = PreloadNearbyFrames(index);
    }

    public void SeekToFrame(int index)
    {
        _isUpdatingSlider = true;
        if (_currentSequence != null)
        {
            _engine.GoToFrame(index);
            _ = GoToFrame(_engine.CurrentIndex);
        }
        _isUpdatingSlider = false;
    }

    private async void OnFolderChanged()
    {
        if (_currentSequence != null)
        {
            var folder = _currentSequence.FolderPath;
            _cache.Clear();
            _currentSequence = null;
            await LoadFolder(folder);
        }
    }

    private async Task PreloadNearbyFrames(int centerIndex)
    {
        if (_currentSequence == null || _isPreloading)
            return;

        _isPreloading = true;

        try
        {
            int total = _currentSequence.TotalFrames;
            int lookAhead = 20;
            int lookBehind = 5;

            var tasks = new List<Task>();

            for (int i = 1; i <= lookAhead; i++)
            {
                int idx = (centerIndex + i) % total;
                var path = _currentSequence.Frames[idx].FilePath;
                if (_cache.Get(path) == null)
                    tasks.Add(_decoder.DecodeAsync(path));
            }

            for (int i = 1; i <= lookBehind; i++)
            {
                int idx = (centerIndex - i + total) % total;
                var path = _currentSequence.Frames[idx].FilePath;
                if (_cache.Get(path) == null)
                    tasks.Add(_decoder.DecodeAsync(path));
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            _isPreloading = false;
        }
    }

    public void SetZoom(double scale)
    {
        ZoomLevel = scale;
        IsFitToWindow = false;
        UpdateZoomText();
    }

    public void FitToWindow()
    {
        ZoomLevel = 1.0;
        IsFitToWindow = true;
        ZoomText = "适配窗口";
    }

    public void OriginalSize()
    {
        ZoomLevel = 1.0;
        IsFitToWindow = false;
        UpdateZoomText();
    }

    private void UpdateZoomText()
    {
        ZoomText = $"{(int)(ZoomLevel * 100)}%";
    }

    private async void OnEngineFrameChanged(int index)
    {
        await GoToFrame(index);
    }

    private void OnEnginePlaybackEnded()
    {
        IsPlaying = false;
    }

    partial void OnFpsChanged(double value)
    {
        _engine.Fps = value;
    }

    partial void OnIsLoopingChanged(bool value)
    {
        _engine.Loop = value;
    }

    [RelayCommand]
    private async Task OpenFolder()
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "选择序列帧文件夹"
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            await LoadFolder(dialog.SelectedPath);
        }
    }

    [RelayCommand]
    private async Task OpenRecentFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
            await LoadFolder(folderPath);
        else
            RecentFolders.Remove(folderPath);
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (_currentSequence == null) return;
        _engine.TogglePlayPause();
        IsPlaying = _engine.State == PlaybackState.Playing;
    }

    [RelayCommand]
    private async Task PreviousFrame()
    {
        if (_currentSequence == null) return;
        _engine.PreviousFrame();
        await GoToFrame(_engine.CurrentIndex);
    }

    [RelayCommand]
    private async Task NextFrame()
    {
        if (_currentSequence == null) return;
        _engine.NextFrame();
        await GoToFrame(_engine.CurrentIndex);
    }

    [RelayCommand]
    private async Task GoToFirstFrame()
    {
        if (_currentSequence == null) return;
        _engine.GoToFrame(0);
        await GoToFrame(0);
    }

    [RelayCommand]
    private async Task GoToLastFrame()
    {
        if (_currentSequence == null) return;
        int last = _currentSequence.TotalFrames - 1;
        _engine.GoToFrame(last);
        await GoToFrame(last);
    }

    [RelayCommand]
    private void FitWindow()
    {
        FitToWindow();
    }

    [RelayCommand]
    private void ZoomOriginal()
    {
        OriginalSize();
    }

    [RelayCommand]
    private async Task Reload()
    {
        if (_currentSequence == null) return;
        _cache.Clear();
        string folder = _currentSequence.FolderPath;
        _currentSequence = null;
        await LoadFolder(folder);
    }

    [RelayCommand]
    private void ShowAbout()
    {
        var about = new Views.AboutWindow();
        about.ShowDialog();
    }
}
