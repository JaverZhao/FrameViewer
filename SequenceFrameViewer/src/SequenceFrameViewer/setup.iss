[Setup]
AppName=FrameView
AppVersion=1.1.0.0
AppPublisher=FrameView
AppVerName=FrameView 1.1.0
DefaultDirName={autopf}\FrameView
DefaultGroupName=FrameView
UninstallDisplayIcon={app}\SequenceFrameViewer.exe
OutputDir=.\installer
OutputBaseFilename=FrameView_Setup_v1.1.0
SolidCompression=yes
Compression=lzma2/ultra
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=Resources\Icons\app.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "publish-selfcontained\SequenceFrameViewer.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\FrameView"; Filename: "{app}\SequenceFrameViewer.exe"
Name: "{group}\卸载 FrameView"; Filename: "{uninstallexe}"
Name: "{commondesktop}\FrameView"; Filename: "{app}\SequenceFrameViewer.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "快捷方式选项:"

[Run]
Filename: "{app}\SequenceFrameViewer.exe"; Description: "运行 FrameView"; Flags: postinstall nowait skipifsilent
