[English](README.md)

<p align="center"><img alt="FrameView" src="icon.png" width="96"></p>

# FrameView

**Windows 轻量化序列帧查看工具。**

快速预览 PNG / JPG / WebP / BMP 序列帧动画，专为视觉特效工作流优化，提供暗色主题界面和完整的播放控制。

---

## 截图

![FrameView 截图](SequenceFrameViewer/other/FrameView_screenshot.png)

---

## 功能

- **打开文件夹 / 拖拽**: 一键打开序列帧文件夹，或拖拽文件夹到窗口
- **自动扫描与自然排序**: 自动识别序列帧并按自然数排序
- **播放控制**: 播放 / 暂停 / 上一帧 / 下一帧 / 首帧 / 尾帧
- **FPS 可调**: 1–120 FPS（默认 24 FPS）
- **循环播放**: 支持循环开关
- **时间轴滑块**: 拖动快速跳帧
- **画布操作**: 鼠标滚轮缩放 / 鼠标拖拽平移
- **适配窗口 / 原始大小**: 一键适配或 1:1 显示
- **透明背景切换**: 棋盘格 / 黑 / 白 / 灰四种模式，便于检查透明区域
- **帧缓存与预读**: LRU 缓存策略，保障播放流畅
- **自动刷新**: 监控文件夹变化，自动重新加载
- **最近打开记录**: 保留最近使用的文件夹
- **键盘快捷键**: 完整键盘导航
- **深色主题**: 护眼深色 UI

---

## 使用方法

1. 点击 **打开文件夹** 或拖拽文件夹到窗口
2. 点击 **▶** 开始播放
3. 滚轮缩放，拖动平移

### 键盘快捷键

| 按键 | 功能 |
|---|---|
| Space | 播放 / 暂停 |
| ← → | 上一帧 / 下一帧 |
| Home / End | 首帧 / 尾帧 |
| Ctrl+O | 打开文件夹 |
| Ctrl+R | 重新加载 |
| F | 适配窗口 |
| 1 | 原始大小 |

---

## 系统要求

- **操作系统**: Windows 7 及以上（x64）
- **运行环境**: .NET 7 Runtime（自包含版本无需安装）

---

## 安装方式

### 方式一 — 安装包（推荐）

从 [Releases](../../releases) 页面下载最新 `FrameView_Setup_v1.1.0.exe` 并运行安装程序。

### 方式二 — 自包含单文件（便携版）

下载 `publish-selfcontained` 版本，无需安装 .NET Runtime，解压后直接运行 `SequenceFrameViewer.exe` 即可。

### 方式三 — 框架依赖（最小体积）

如已安装 .NET 7 Runtime，可使用 `publish` 版本（约 1 MB）。

---

## 源码构建

### 前置要求

- [.NET 7 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/7.0)
- Windows 操作系统（WPF 依赖）

```powershell
# 构建 Debug
dotnet build

# 构建 Release
dotnet build -c Release

# 发布：框架依赖（~1 MB，需 .NET 7 Runtime）
dotnet publish -c Release -o publish

# 发布：自包含单文件（~153 MB，零依赖）
dotnet publish -c Release --self-contained -r win-x64 `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:DebugType=none -p:DebugSymbols=false -o publish-selfcontained
```

### 制作安装包

执行自包含发布后，运行 Inno Setup 脚本：

```powershell
ISCC.exe setup.iss
```

需要安装 [Inno Setup 6](https://jrsoftware.org/isdl.php)。

---

## 技术栈

| 层 | 技术 |
|---|---|
| 语言 | C# |
| 框架 | .NET 7（WPF + Windows Forms） |
| 架构 | MVVM |
| MVVM 工具包 | CommunityToolkit.Mvvm 8.2.2 |
| 图片解码 | Windows Imaging Component (WIC)，原生解码 |
| UI | XAML，深色主题 |
| 安装包 | Inno Setup 6 |

---

## 许可

[MIT](../LICENSE)
