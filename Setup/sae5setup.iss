#define MyAppName "Stitch Art Easy! 5"
#define MyAppVersion "5.1.1"
#define MyAppFileVersion "5.1.1.29"
#define MyAppPublisher "Mykola Kovalchuk"
#define MyAppCopyright "Copyright © Mykola Kovalchuk"
#define MyAppURL "https://mykola.xyz/stitcharteasy/"
#define MyAppExeName "SAE5.exe"

#include "InnoDependencyInstaller\setup.iss"

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
LicenseFile=C:\SAE5\Setup\license.txt
OutputBaseFilename=sae5setup
Compression=lzma
SolidCompression=yes
ChangesAssociations=yes
WizardImageFile=compiler:wizmodernimage-is.bmp
WizardSmallImageFile=compiler:wizmodernsmallimage-is.bmp
RestartIfNeededByRun=False
CloseApplicationsFilter=*.exe,*.dll
VersionInfoVersion={#MyAppFileVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Cross-stitch embroidery schemes designer
VersionInfoTextVersion={#MyAppVersion}
VersionInfoCopyright=2003-2022 © {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppFileVersion}

//MinVersion=6.1sp1
PrivilegesRequired=admin

// remove next line if you only deploy 32-bit binaries and dependencies
ArchitecturesInstallIn64BitMode=x64

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
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\Apps\SAE5.Win\bin\Release\SAE5.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Apps\SAE5.Win\bin\Release\SAE5.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Apps\SAE5.Win\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "..\Resources\Images\Samples\*"; DestDir: "{app}\Samples"; Flags: ignoreversion
Source: "..\Ravlyk.SAE\Ravlyk.SAE.Resources\Fonts\Ravlyk.Znaky.2.ttf"; DestDir: "{commonfonts}"; FontInstall: Ravlyk.Znaky.2; Flags: uninsneveruninstall onlyifdoesntexist
Source: "..\Resources\Icons\thimble_doc.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Resources\Icons\thimble.ico"; DestDir: "{app}"; Flags: ignoreversion
// NOTE: Don't use "Flags: ignoreversion" on any shared system files

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
  // https://dotnet.microsoft.com/download/dotnet-framework/net48
  if not IsDotNetInstalled(net48, 0) then begin
    AddDependency('dotnetfx48.exe',
      '/lcid ' + IntToStr(GetUILanguage) + ' /passive /norestart',
      '.NET Framework 4.8',
      'https://download.visualstudio.microsoft.com/download/pr/7afca223-55d2-470a-8edc-6a1739ae3252/c9b8749dd99fc0d4453b2a3e4c37ba16/ndp48-web.exe',
      '', False, False, False);
  end;

  Result := True;
end;
