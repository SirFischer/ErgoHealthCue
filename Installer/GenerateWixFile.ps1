# Generate WiX Product.wxs with all published files
param(
    [string]$PublishPath = "..\ErgoHealthCue\bin\Release\net8.0-windows\win-x64\publish",
    [string]$OutputPath = "Product.wxs",
    [string]$Version = "1.0.0"
)

Write-Host "Generating WiX installer definition..."
Write-Host "Publish Path: $PublishPath"
Write-Host "Version: $Version"

# Get all files from publish directory (excluding subdirectories for now)
$files = Get-ChildItem -Path $PublishPath -File | Where-Object { $_.Extension -ne ".pdb" }

Write-Host "Found $($files.Count) files to include in installer"

# Generate component entries for all DLL and other files
$fileComponents = @()
$fileRefs = @()
$fileId = 1

foreach ($file in $files) {
    if ($file.Name -eq "ErgoHealthCue.exe") {
        continue # Skip exe as it's handled separately
    }
    
    $componentId = "File_$fileId"
    $fileIdName = "File_$fileId"
    $fileName = $file.Name
    
    $fileComponents += @"
        <Component Id="$componentId" Bitness="always64">
          <File Id="$fileIdName" Source="$PublishPath\$fileName" KeyPath="yes" />
        </Component>
"@
    
    $fileRefs += "      <ComponentRef Id=`"$componentId`" />"
    $fileId++
}

# Generate the complete Product.wxs
$wxsContent = @"
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">
  <Package 
    Name="ErgoHealthCue" 
    Manufacturer="ErgoHealthCue"
    Version="$Version" 
    UpgradeCode="A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"
    Language="1033"
    Codepage="1252">
    
    <SummaryInformation 
      Keywords="Installer"
      Description="ErgoHealthCue Installer"
      Manufacturer="ErgoHealthCue" />

    <!-- Major Upgrade Configuration -->
    <MajorUpgrade 
      DowngradeErrorMessage="A newer version of ErgoHealthCue is already installed."
      AllowDowngrades="no"
      AllowSameVersionUpgrades="no" />

    <!-- Media Definition -->
    <Media Id="1" Cabinet="ErgoHealthCue.cab" EmbedCab="yes" />

    <!-- Installation Directory Structure -->
    <StandardDirectory Id="LocalAppDataFolder">
      <Directory Id="INSTALLFOLDER" Name="ErgoHealthCue">
        <!-- Main Executable -->
        <Component Id="MainExecutable" Bitness="always64">
          <File 
            Id="ErgoHealthCueExe" 
            Name="ErgoHealthCue.exe" 
            Source="$PublishPath\ErgoHealthCue.exe"
            KeyPath="yes">
            <Shortcut 
              Id="StartMenuShortcut"
              Directory="ProgramMenuFolder"
              Name="ErgoHealthCue"
              Description="Ergonomic health reminder application"
              WorkingDirectory="INSTALLFOLDER"
              Icon="ErgoHealthCueIcon"
              IconIndex="0"
              Advertise="yes" />
            <Shortcut 
              Id="DesktopShortcut"
              Directory="DesktopFolder"
              Name="ErgoHealthCue"
              Description="Ergonomic health reminder application"
              WorkingDirectory="INSTALLFOLDER"
              Icon="ErgoHealthCueIcon"
              IconIndex="0"
              Advertise="yes" />
          </File>
        </Component>
        
        <!-- All other application files -->
$($fileComponents -join "`n        ")
      </Directory>
    </StandardDirectory>

    <!-- Desktop Folder -->
    <StandardDirectory Id="DesktopFolder" />
    
    <!-- Start Menu Folder -->
    <StandardDirectory Id="ProgramMenuFolder" />

    <!-- Icon for shortcuts and Add/Remove Programs -->
    <Icon Id="ErgoHealthCueIcon" SourceFile="$PublishPath\ErgoHealthCue.exe" />
    
    <!-- Add/Remove Programs Icon -->
    <Property Id="ARPPRODUCTICON" Value="ErgoHealthCueIcon" />
    
    <!-- Add/Remove Programs Information -->
    <Property Id="ARPHELPLINK" Value="https://github.com/SirFischer/ErgoHealthCue" />
    <Property Id="ARPURLINFOABOUT" Value="https://github.com/SirFischer/ErgoHealthCue" />
    <Property Id="ARPNOREPAIR" Value="yes" Secure="yes" />
    
    <!-- Features to Install -->
    <Feature Id="Complete" Level="1">
      <ComponentRef Id="MainExecutable" />
$($fileRefs -join "`n")
    </Feature>
  </Package>
</Wix>
"@

# Write to file
Set-Content -Path $OutputPath -Value $wxsContent -Encoding UTF8

Write-Host "Generated $OutputPath successfully!"
Write-Host "Included $($files.Count) files in the installer"
