using System.Collections.Generic;

namespace SequenceFrameViewer.Models;

public class AppSettings
{
    public int DefaultFps { get; set; } = 24;
    public bool LoopPlayback { get; set; } = true;
    public string BackgroundMode { get; set; } = "Checkerboard";
    public string Language { get; set; } = "zh";
    public string Theme { get; set; } = "Dark";
    public List<string> RecentFolders { get; set; } = new();
    public int MaxCacheMemoryMb { get; set; } = 512;
    public double LastWindowWidth { get; set; } = 1200;
    public double LastWindowHeight { get; set; } = 800;
}
