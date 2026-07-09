using System.Collections.Generic;
using System.ComponentModel;

namespace SequenceFrameViewer.Resources;

public class LocalizationService : INotifyPropertyChanged
{
    public static LocalizationService Default { get; } = new();

    private string _culture = "zh";

    private static readonly Dictionary<string, string> Zh = new()
    {
        ["Open"] = "打开",
        ["OpenFolderTooltip"] = "打开文件夹 (Ctrl+O)",
        ["RecentFoldersTooltip"] = "最近打开",
        ["RecentFoldersPlaceholder"] = "最近打开...",
        ["ReloadTooltip"] = "重新加载 (Ctrl+R)",
        ["BackgroundModeTooltip"] = "背景模式",
        ["FpsLabel"] = "FPS",
        ["LoopTooltip"] = "循环播放",
        ["FitWindowTooltip"] = "适配窗口 (F)",
        ["OriginalSizeTooltip"] = "原始大小 (1)",
        ["AboutTooltip"] = "关于",
        ["FirstFrameTooltip"] = "首帧 (Home)",
        ["PrevFrameTooltip"] = "上一帧 (←)",
        ["PlayPauseTooltip"] = "播放 / 暂停 (Space)",
        ["NextFrameTooltip"] = "下一帧 (→)",
        ["LastFrameTooltip"] = "尾帧 (End)",
        ["FrameLabel"] = "帧",
        ["AboutTitle"] = "关于 FrameView",
        ["VersionFormat"] = "版本 {0}",
        ["AppDescription"] = "Windows 轻量化序列帧查看工具",
        ["AboutDescription"] = "快速预览 PNG / JPG / WebP / BMP 序列帧动画。\n支持播放控制、缩放、透明背景、循环播放等功能。",
        ["Ok"] = "确定",
        ["EmptyState"] = "拖入序列帧文件夹开始预览",
        ["NoImagesFound"] = "文件夹中没有支持的图片文件",
        ["SelectFolder"] = "选择序列帧文件夹",
        ["UnhandledErrorFormat"] = "发生未处理的异常:\n{0}",
        ["ErrorTitle"] = "FrameView - 错误",
        ["FitWindow"] = "适配窗口",
        ["OriginalSize"] = "原始大小",
        ["Checkerboard"] = "棋盘格",
        ["Black"] = "黑色",
        ["White"] = "白色",
        ["Gray"] = "灰色",
    };

    private static readonly Dictionary<string, string> En = new()
    {
        ["Open"] = "Open",
        ["OpenFolderTooltip"] = "Open Folder (Ctrl+O)",
        ["RecentFoldersTooltip"] = "Recent Folders",
        ["RecentFoldersPlaceholder"] = "Recent Folders...",
        ["ReloadTooltip"] = "Reload (Ctrl+R)",
        ["BackgroundModeTooltip"] = "Background Mode",
        ["FpsLabel"] = "FPS",
        ["LoopTooltip"] = "Loop Playback",
        ["FitWindowTooltip"] = "Fit to Window (F)",
        ["OriginalSizeTooltip"] = "Original Size (1)",
        ["AboutTooltip"] = "About",
        ["FirstFrameTooltip"] = "First Frame (Home)",
        ["PrevFrameTooltip"] = "Previous Frame (←)",
        ["PlayPauseTooltip"] = "Play / Pause (Space)",
        ["NextFrameTooltip"] = "Next Frame (→)",
        ["LastFrameTooltip"] = "Last Frame (End)",
        ["FrameLabel"] = "Frame",
        ["AboutTitle"] = "About FrameView",
        ["VersionFormat"] = "Version {0}",
        ["AppDescription"] = "Lightweight Windows sequence frame viewer",
        ["AboutDescription"] = "Quick preview of PNG / JPG / WebP / BMP sequence animations.\nSupports playback controls, zoom, transparent background, loop playback and more.",
        ["Ok"] = "OK",
        ["EmptyState"] = "Drag a folder to start previewing",
        ["NoImagesFound"] = "No supported image files found in the folder",
        ["SelectFolder"] = "Select Sequence Frame Folder",
        ["UnhandledErrorFormat"] = "An unhandled exception occurred:\n{0}",
        ["ErrorTitle"] = "FrameView - Error",
        ["FitWindow"] = "Fit to Window",
        ["OriginalSize"] = "Original Size",
        ["Checkerboard"] = "Checkerboard",
        ["Black"] = "Black",
        ["White"] = "White",
        ["Gray"] = "Gray",
    };

    private Dictionary<string, string> Strings => _culture == "en" ? En : Zh;

    public string this[string key] => Strings.TryGetValue(key, out var val) ? val : key;

    public string Culture => _culture;

    public string Open => Strings["Open"];
    public string OpenFolderTooltip => Strings["OpenFolderTooltip"];
    public string RecentFoldersTooltip => Strings["RecentFoldersTooltip"];
    public string RecentFoldersPlaceholder => Strings["RecentFoldersPlaceholder"];
    public string ReloadTooltip => Strings["ReloadTooltip"];
    public string BackgroundModeTooltip => Strings["BackgroundModeTooltip"];
    public string FpsLabel => Strings["FpsLabel"];
    public string LoopTooltip => Strings["LoopTooltip"];
    public string FitWindowTooltip => Strings["FitWindowTooltip"];
    public string OriginalSizeTooltip => Strings["OriginalSizeTooltip"];
    public string AboutTooltip => Strings["AboutTooltip"];
    public string FirstFrameTooltip => Strings["FirstFrameTooltip"];
    public string PrevFrameTooltip => Strings["PrevFrameTooltip"];
    public string PlayPauseTooltip => Strings["PlayPauseTooltip"];
    public string NextFrameTooltip => Strings["NextFrameTooltip"];
    public string LastFrameTooltip => Strings["LastFrameTooltip"];
    public string FrameLabel => Strings["FrameLabel"];
    public string AboutTitle => Strings["AboutTitle"];
    public string VersionFormat => Strings["VersionFormat"];
    public string AppDescription => Strings["AppDescription"];
    public string AboutDescription => Strings["AboutDescription"];
    public string Ok => Strings["Ok"];
    public string EmptyState => Strings["EmptyState"];
    public string NoImagesFound => Strings["NoImagesFound"];
    public string SelectFolder => Strings["SelectFolder"];
    public string UnhandledErrorFormat => Strings["UnhandledErrorFormat"];
    public string ErrorTitle => Strings["ErrorTitle"];
    public string FitWindow => Strings["FitWindow"];
    public string OriginalSize => Strings["OriginalSize"];
    public string Checkerboard => Strings["Checkerboard"];
    public string Black => Strings["Black"];
    public string White => Strings["White"];
    public string Gray => Strings["Gray"];

    public void SetCulture(string culture)
    {
        if (_culture == culture) return;
        _culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
