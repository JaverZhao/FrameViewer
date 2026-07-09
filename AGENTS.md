# FrameView — AGENTS.md

## 项目结构

单项目 .NET 7 WPF 桌面应用，一个 `.sln` + 一个 `.csproj`。

```
SequenceFrameViewer/
├── SequenceFrameViewer.sln
├── src/SequenceFrameViewer/     # 唯一项目
│   ├── App.xaml(.cs)            # 入口
│   ├── MainWindow.xaml(.cs)     # 主窗口
│   ├── Models/                  # AppSettings, FrameItem, FrameSequence
│   ├── ViewModels/              # MainViewModel (单一 VM)
│   ├── Services/                # 7 个服务类
│   ├── Views/                   # AboutWindow
│   ├── Helpers/                 # 值转换器, NaturalSortComparer
│   ├── Resources/               # Icons (SVG+ico), Themes (DarkTheme.xaml)
│   ├── setup.iss                # Inno Setup 安装包脚本
│   ├── publish/                 # 框架依赖发布产物 (gitignored)
│   ├── publish-selfcontained/   # 自包含单文件发布产物 (gitignored)
│   └── installer/               # 安装包输出 (gitignored)
└── tests/                       # 空目录，无测试
```

## 构建与发布命令

```powershell
# 构建 Debug
dotnet build

# 构建 Release
dotnet build -c Release

# 发布：框架依赖（~1MB，需 .NET 7 Runtime）
dotnet publish -c Release -o publish

# 发布：自包含单文件（~153MB，零依赖）
dotnet publish -c Release --self-contained -r win-x64 `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:DebugType=none -p:DebugSymbols=false -o publish-selfcontained

# 制作安装包（先执行自包含发布，再运行此脚本）
ISCC.exe setup.iss
```

- 安装包需要 `publish-selfcontained/SequenceFrameViewer.exe` 预存在
- 以上产物目录均在 `.gitignore` 中

## 技术约束

- **Windows-only**: 目标 `net7.0-windows`，WPF + WinForms 混合
- **框架**: CommunityToolkit.Mvvm 8.2.2，WIC 原生图片解码
- **Nullable**: 启用 (`<Nullable>enable</Nullable>`)
- **测试**: `tests/` 目录为空，无测试框架
- **CI/CD**: 无 GitHub Actions 或其他 CI 配置
- **无 linter/formatter 配置**: .NET 编译器本身承担此角色

## 分支

- `master` 和 `main` 内容已合并同步
- 主要开发在 `master`

## 版本

当前版本 1.1.0.0，记录在 `.csproj` 的 `Version/FileVersion/AssemblyVersion`。

## 关键路径注意事项

- 路径含中文/全角字符时用 WIC 原生解码，需 `Uri` 而非字符串传参（历史修复过此 bug）
- `FrameCache` 使用 LRU 策略，并发访问需小心（历史 NullReferenceException 修复）
- `LogService` 输出目标未显式配置，写入文件位置需读源码确认
- 应用图标路径：`Resources\Icons\app.ico`
