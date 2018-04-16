; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "EDDI"
#define MyAppVersion "3.0.0"
#define MyAppPublisher "Elite Dangerous Community Developers (EDCD)"
#define MyAppURL "https://github.com/EDCD/EDDI/"
#define MyAppExeName "EDDI.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{830C0324-30D8-423C-B5B4-D7EE8D007A79}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={reg:HKCU\Software\VoiceAttack.com\VoiceAttack,InstallPath|{pf32}\VoiceAttack}\Apps\{#MyAppName}
DefaultGroupName={#MyAppName}
SourceDir="{#SourcePath}\bin\Release"
OutputDir="{#SourcePath}\bin\Installer"
OutputBaseFilename={#MyAppName}-{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardImageFile={#SourcePath}\graphics\logo.bmp
WizardSmallImageFile={#SourcePath}\graphics\logo.bmp
UninstallDisplayIcon={app}\EDDI\EDDI.ico
UsePreviousTasks=False
DisableWelcomePage=False
DisableDirPage=no
AllowUNCPath=False

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "EDDI.exe"; DestDir: "{app}"; Flags: ignoreversion

Source: "x86\*.*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs replacesameversion
Source: "*.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

Source: "eddi.json"; DestDir: "{app}"; Flags: ignoreversion

Source: "ChangeLog.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "Help.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "Variables.md"; DestDir: "{app}"; Flags: ignoreversion

Source: "EDDI.vap"; DestDir: "{app}"; Flags: ignoreversion

Source: "EddiDataProviderService.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "EDDI.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "EddiVoiceAttackResponder.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "EddiGalnetMonitor.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Data.SQLite.dll.config"; DestDir: "{app}"; Flags: ignoreversion

; Remove outdated files
[InstallDelete]
Type: files; Name: "{app}\Eddi.exe"
Type: files; Name: "{app}\EDDI.ico"
Type: files; Name: "{app}\EddiNetLogMonitor.dll"
Type: files; Name: "{app}\Newtonsoft.Json.xml"
Type: files; Name: "{app}\CommonMark.xml"
Type: files; Name: "{app}\Exceptionless.Wpf.xml"
Type: files; Name: "{app}\Exceptionless.xml"
Type: files; Name: "{app}\MathNet.Numerics.xml"
Type: files; Name: "{app}\System.Data.SQLite.xml"
Type: files; Name: "{app}\SimpleFeedReader.xml"
Type: files; Name: "{app}\CSCore.xml"
Type: files; Name: "{app}\RestSharp.xml"
Type: files; Name: "{app}\EntityFramework.SqlServer.xml"
Type: files; Name: "{app}\EntityFramework.xml"

; Remove sensitive data on uninstall
[UninstallDelete]
Type: files; Name: "{userappdata}\EDDI\credentials.json"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[ThirdParty]
UseRelativePaths=True

[Messages]
SelectDirBrowseLabel=To continue, click Next. If this is not your VoiceAttack installation location, or you would like to put the EDDI files in a different location, click Browse.
