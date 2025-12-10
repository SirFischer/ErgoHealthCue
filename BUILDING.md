# Building ErgoHealthCue

This document explains how to build ErgoHealthCue from source and create the MSI installer.

## Prerequisites

- .NET 8.0 SDK or later
- Windows OS (for building the MSI installer)
- WiX Toolset v5 (for creating MSI installer)

## Building the Application

### Quick Build

```bash
dotnet build ErgoHealthCue/ErgoHealthCue.csproj --configuration Release
```

### Publishing for Distribution

To create a self-contained application:

```bash
dotnet publish ErgoHealthCue/ErgoHealthCue.csproj -c Release -r win-x64 --self-contained true
```

The published files will be in: `ErgoHealthCue/bin/Release/net8.0-windows/win-x64/publish/`

## Building the MSI Installer

### Install WiX Toolset

```bash
dotnet tool install --global wix --version 5.0.2
```

### Build Steps

1. First, publish the application:

```bash
dotnet publish ErgoHealthCue/ErgoHealthCue.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false
```

2. Build the installer:

```bash
cd Installer
wix build -arch x64 Product.wxs -o ErgoHealthCue-1.0.0-x64.msi
```

The MSI installer will be created in the `Installer` directory.

## Automated Release via GitHub Actions

The project includes a GitHub Actions workflow that automatically builds and creates releases:

### Creating a Release

#### Method 1: Tag-based Release (Recommended)

```bash
git tag v1.0.0
git push origin v1.0.0
```

The workflow will automatically:
1. Build the application
2. Create the MSI installer
3. Create a GitHub release
4. Upload the MSI as a release asset

#### Method 2: Manual Workflow Dispatch

1. Go to the repository on GitHub
2. Navigate to Actions → Build and Release
3. Click "Run workflow"
4. Enter the version number (e.g., 1.0.0)
5. Click "Run workflow"

## Version Management

Version numbers are managed in three files:

1. `ErgoHealthCue/ErgoHealthCue.csproj` - Application version
2. `Installer/Product.wxs` - Installer version
3. `Installer/Installer.wixproj` - Installer project version

The GitHub Actions workflow automatically updates all version numbers based on the git tag or manual input.

## Testing the Installer

After building the MSI:

1. **Install**: Double-click the MSI file to install
2. **Verify**: Check that:
   - Desktop shortcut was created
   - Start Menu entry exists
   - Application runs correctly
3. **Upgrade**: Build a new version with incremented version number and install over existing installation
4. **Uninstall**: Use Windows "Add or Remove Programs" to uninstall

## Troubleshooting

### WiX Build Errors

If you encounter WiX build errors:

1. Ensure all referenced files exist in the publish directory
2. Check that file paths in `Product.wxs` are correct
3. Verify WiX Toolset is properly installed: `wix --version`

### Missing Dependencies

If the installer fails to include all dependencies:

- Ensure `PublishSingleFile=false` is set
- Check the publish output directory for all DLLs
- Update `Product.wxs` to include any missing files

### Version Conflicts

If you get version conflict errors during installation:

- Ensure the version number is higher than the currently installed version
- Or completely uninstall the old version first
- Check that the UpgradeCode in `Product.wxs` remains consistent
