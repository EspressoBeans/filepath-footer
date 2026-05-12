# Filepath Footer

A Visual Studio extension that displays the **full file path** of the currently active document in a thin bar at the bottom of every editor window.

<img width="1577" height="976" alt="demo of filepath on footer" src="https://github.com/user-attachments/assets/0049a074-469f-4853-8b37-364ce3d2b022" />


## Features

- **Full file path** — the complete path (directory, filename, and extension) of the open file is shown below the horizontal scroll bar, in the editor's themed status-bar colour (adapts to Light / Dark / Blue themes automatically).
- **One-click copy** — click anywhere on the path to copy it to the clipboard.
- **Transient confirmation** — the bar briefly shows **✓ File path copied to clipboard** in green, then restores the path after 2 seconds.
- **Live updates** — the path refreshes automatically when a file is renamed or saved-as.
- **Theming** - Observes the theme applied to Visual Studio.

## Requirements

| Requirement | Version |
|---|---|
| Visual Studio | 2022 (17.0) or later — Community, Professional, or Enterprise |
| .NET Framework | 4.8 |
| Architecture | x64 (amd64) |

## Building

1. Open `FilepathFooter.sln` in Visual Studio.
2. Restore NuGet packages (automatic on first build).
3. Build → the `.vsix` file is emitted to `FilepathFooter\bin\<Configuration>\net48\`.

> **Note:** The project uses an SDK-style `.csproj` with custom MSBuild targets (`SetVsixProperties`, `BuildVsix`) to work around evaluation-order differences between the .NET SDK and the VSSDK build tooling.

## Installing

Double-click the generated `.vsix` file, or go to  
**Extensions → Manage Extensions → Install from VSIX…**

## Project Structure

```
FilepathFooter/
├── Editor/
│   ├── FilepathFooterMargin.cs          # WPF Grid margin — renders the path and handles click-to-copy
│   └── FilepathFooterMarginFactory.cs   # MEF factory that wires the margin into VS
├── FilepathFooterPackage.cs             # AsyncPackage entry point (registration only)
├── source.extension.vsixmanifest        # VSIX metadata, installation targets, and assets
└── FilepathFooter.csproj                # SDK-style project with VSSDK integration
```

## Author

Vic Guadalupe
