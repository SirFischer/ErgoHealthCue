# Build MSI Installer for ErgoHealthCue
# This script builds the MSI installer without requiring Visual Studio or WiX SDK

param(
    [string]$Version = "1.0.0",
    [switch]$SkipPublish = $false
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building ErgoHealthCue MSI Installer" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Navigate to repository root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
Set-Location $repoRoot

# Step 1: Publish the application
if (-not $SkipPublish) {
    Write-Host "`n[1/4] Publishing application..." -ForegroundColor Yellow
    dotnet publish ErgoHealthCue/ErgoHealthCue.csproj `
        -c Release `
        -r win-x64 `
        --self-contained true `
        -p:PublishSingleFile=false `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:PublishReadyToRun=true
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to publish application"
        exit 1
    }
    Write-Host "Application published successfully!" -ForegroundColor Green
} else {
    Write-Host "`n[1/4] Skipping publish (using existing build)..." -ForegroundColor Yellow
}

# Step 2: Check if WiX is installed
Write-Host "`n[2/4] Checking WiX installation..." -ForegroundColor Yellow
$wixPath = Get-Command wix -ErrorAction SilentlyContinue

if (-not $wixPath) {
    Write-Host "WiX not found. Installing WiX Toolset..." -ForegroundColor Yellow
    dotnet tool install --global wix --version 5.0.2
    
    # Refresh PATH
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path", "User")
    
    $wixPath = Get-Command wix -ErrorAction SilentlyContinue
    if (-not $wixPath) {
        Write-Error "Failed to install WiX. Please install manually: dotnet tool install --global wix"
        exit 1
    }
}

Write-Host "WiX version:" -ForegroundColor Green
wix --version

# Step 3: Generate Product.wxs
Write-Host "`n[3/4] Generating WiX product definition..." -ForegroundColor Yellow
Set-Location "$repoRoot\Installer"
.\GenerateWixFile.ps1 -Version $Version
Write-Host "Product.wxs generated successfully!" -ForegroundColor Green

# Step 4: Build MSI
Write-Host "`n[4/4] Building MSI installer..." -ForegroundColor Yellow
$outputMsi = "ErgoHealthCue-$Version-x64.msi"

wix build -arch x64 Product.wxs -o $outputMsi

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build MSI installer"
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "MSI Installer built successfully!" -ForegroundColor Green
Write-Host "Location: $repoRoot\Installer\$outputMsi" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# Return to original directory
Set-Location $repoRoot
