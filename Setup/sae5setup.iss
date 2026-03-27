// Inno Setup Script, version 6.7.1

#define MyAppName "Stitch Art Easy! 5"
#define MyAppVersion "5.1.3"
#define MyAppFileVersion "5.1.3.31"
#define MyAppPublisher "Mykola Kovalchuk"
#define MyAppCopyright "Mykola Kovalchuk (C) 2003-2026"
#define MyAppURL "https://github.com/MykolaKovalchuk/SAE5"
#define MyAppExeName "SAE5.exe"

#include "InnoDependencyInstaller\CodeDependencies.iss"

[Setup]
// NOTE: The value of AppId uniquely identifies this application.
// Do not use the same AppId value in installers for other applications.
AppId={{455402E2-9688-4441-958D-05831236F54D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright={#MyAppCopyright}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=license.txt
OutputBaseFilename=sae5setup
Compression=lzma
SolidCompression=yes
ChangesAssociations=yes
WizardImageFile=compiler:WizClassicImage-IS.bmp
WizardSmallImageFile=compiler:WizClassicSmallImage-IS.bmp
RestartIfNeededByRun=False
CloseApplicationsFilter=*.exe,*.dll
VersionInfoVersion={#MyAppFileVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Cross-stitch embroidery schemes designer
VersionInfoTextVersion={#MyAppVersion}
VersionInfoCopyright=2003-2026 (C) {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppFileVersion}

//MinVersion=6.1sp1
PrivilegesRequired=admin

// remove next line if you only deploy 32-bit binaries and dependencies
ArchitecturesInstallIn64BitMode=x64os

// dependency installation requires ready page and ready memo to be enabled (default behaviour)
DisableReadyPage=no
DisableReadyMemo=no

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "uk"; MessagesFile: "compiler:Languages\Ukrainian.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\Apps\SAE5.Win\bin\Release\net10.0-windows\SAE5.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Apps\SAE5.Win\bin\Release\net10.0-windows\*.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Apps\SAE5.Win\bin\Release\net10.0-windows\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Apps\SAE5.Win\bin\Release\net10.0-windows\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "..\Resources\Images\Samples\*"; DestDir: "{app}\Samples"; Flags: ignoreversion
Source: "..\Ravlyk.SAE\Ravlyk.SAE.Resources\Fonts\Ravlyk.Znaky.2.ttf"; DestDir: "{commonfonts}"; FontInstall: Ravlyk.Znaky.2; Flags: uninsneveruninstall onlyifdoesntexist
Source: "..\Resources\Icons\thimble_doc.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Resources\Icons\thimble.ico"; DestDir: "{app}"; Flags: ignoreversion
// NOTE: Do not use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKCR; SubKey: .SA4; ValueType: string; ValueData: Stitch Art Easy! document; Flags: uninsdeletekey
Root: HKCR; SubKey: Stitch Art Easy! document; ValueType: string; ValueData: Cross-stitch scheme; Flags: uninsdeletekey
Root: HKCR; SubKey: Stitch Art Easy! document\Shell\Open\Command; ValueType: string; ValueData: """{app}\SAE5.exe"" ""%1"""; Flags: uninsdeletevalue
Root: HKCR; Subkey: Stitch Art Easy! document\DefaultIcon; ValueType: string; ValueData: {app}\thimble_doc.ico; Flags: uninsdeletevalue

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet100Desktop;

  Result := True;
end;
