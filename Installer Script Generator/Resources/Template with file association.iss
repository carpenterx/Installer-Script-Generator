#define AppName "APP_NAME"
#define AppExeName "APP_NAME.exe"
#define ReleasePath "RELEASE_PATH"

[Setup]
ChangesAssociations=yes
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

[Registry]
Root: HKA; Subkey: "Software\Classes\.[EXTENSION]\OpenWith{#AppName}"; ValueType: string; ValueName: "My[FILE_TYPE]File.[EXTENSION]"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\My[FILE_TYPE]File.[EXTENSION]"; ValueType: string; ValueName: ""; ValueData: "My [FILE_TYPE] File"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\My[FILE_TYPE]File.[EXTENSION]\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#AppExeName},0"
Root: HKA; Subkey: "Software\Classes\My[FILE_TYPE]File.[EXTENSION]\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExeName}"" ""%1"""
Root: HKA; Subkey: "Software\Classes\Applications\{#AppExeName}\SupportedTypes"; ValueType: string; ValueName: ".[EXTENSION]"; ValueData: ""