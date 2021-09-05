#define AppName "APP_NAME"
#define AppExeName "APP_NAME.exe"
#define ReleasePath "RELEASE_PATH"

[Setup]
AppName={#AppName}
AppVersion=APP_VERSION
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
UninstallDisplayIcon={app}\{#AppExeName}
Compression=lzma2
SolidCompression=yes
OutputDir="{#AppName} Installer"
OutputBaseFilename="{#AppName} Installer"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; \
    GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "{#ReleasePath}\*"; DestDir: "{app}\"; Flags: ignoreversion recursesubdirs;

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon
