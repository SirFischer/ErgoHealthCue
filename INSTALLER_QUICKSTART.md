# Quick Start Guide - Building the MSI Installer

This guide will help you build the MSI installer for ErgoHealthCue in just a few simple steps.

## Prerequisites

You only need:
- Windows 10 or later
- PowerShell (comes with Windows)
- .NET 8.0 SDK ([download here](https://dotnet.microsoft.com/download/dotnet/8.0))

**Note:** You do NOT need Visual Studio or any special SDKs installed!

## Build Steps

1. **Open PowerShell**
   - Press `Win + X` and select "Windows PowerShell" or "Terminal"

2. **Navigate to the repository**
   ```powershell
   cd path\to\ErgoHealthCue
   ```

3. **Run the build script**
   ```powershell
   cd Installer
   .\BuildInstaller.ps1 -Version "1.0.0"
   ```

4. **Done!**
   - The MSI installer will be created at: `Installer\ErgoHealthCue-1.0.0-x64.msi`

## What the Script Does

The build script automatically:
1. Publishes the application with all dependencies
2. Installs WiX Toolset (if not already installed)
3. Generates a complete list of all files to include
4. Builds the MSI installer

## Troubleshooting

### "Cannot be loaded because running scripts is disabled"

If you see this error, run:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Then try the build command again.

### Build fails with "dotnet not found"

Install the .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0

## Updating the Version

To create a new version:

1. Update the version in `ErgoHealthCue/ErgoHealthCue.csproj`:
   ```xml
   <Version>1.0.1</Version>
   <AssemblyVersion>1.0.1.0</AssemblyVersion>
   <FileVersion>1.0.1.0</FileVersion>
   ```

2. Run the build script with the new version:
   ```powershell
   .\BuildInstaller.ps1 -Version "1.0.1"
   ```

## Creating a GitHub Release

1. Build the MSI installer
2. Go to: https://github.com/SirFischer/ErgoHealthCue/releases/new
3. Create a new tag (e.g., `v1.0.0`)
4. Fill in the release title and description
5. Upload the MSI file
6. Click "Publish release"

Users can then download and install directly from the Releases page!

## Testing the Installer

Before releasing:

1. **Install**: Double-click the MSI to install
2. **Check**: 
   - Desktop shortcut should appear
   - Start Menu entry should exist
   - Application should run from shortcuts
3. **Uninstall**: Use "Add or Remove Programs" to uninstall cleanly
4. **Upgrade**: Build a newer version and install over the old one

That's it! The installer is production-ready.
