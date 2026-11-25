<#
.SYNOPSIS
    Builds the template NuGet package locally and installs it so it appears in
    the Visual Studio "Create a new project" dialog.

.DESCRIPTION
    1. Uninstalls any previous version of this template (folder-based or package).
    2. Clears the dotnet new (template engine) cache so changes are picked up.
    3. Packs Jackobo.WebApiTemplate.Pack.csproj into a local .nupkg (in the nupkg folder).
    4. Installs that .nupkg via `dotnet new install`.
    5. Rebuilds the template engine cache.

    The package is NOT published anywhere; it stays on this machine.

    NOTE: The cache that is cleared here belongs to the .NET template engine
    (the engine behind `dotnet new`). Visual Studio loads its template list at
    startup and keeps it in memory, so you must STILL restart Visual Studio
    after running this script for the changes to show up in the IDE. No script
    can force a running Visual Studio instance to reload templates.

.PARAMETER Version
    Optional package version. Defaults to the version in the .csproj.

.PARAMETER SkipCacheReset
    Skips clearing/rebuilding the template engine cache (faster, but changes
    may not be picked up reliably).

.PARAMETER HardReset
    Nuclear option: deletes the entire %USERPROFILE%\.templateengine folder
    before reinstalling. This also clears the registration of every OTHER
    template you installed via `dotnet new install`, so use only when the
    normal cache reset does not help (e.g. a corrupted cache).

.EXAMPLE
    .\pack-and-install.ps1

.EXAMPLE
    .\pack-and-install.ps1 -Version 1.0.1

.EXAMPLE
    .\pack-and-install.ps1 -HardReset
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [switch]$SkipCacheReset,

    [Parameter(Mandatory = $false)]
    [switch]$HardReset
)

$ErrorActionPreference = "Stop"

$root       = $PSScriptRoot
$packProj   = Join-Path $root "Jackobo.WebApiTemplate.Pack.csproj"
$nupkgDir   = Join-Path $root "nupkg"
$packageId  = "Jackobo.WebApiTemplate"
$teDir      = Join-Path $env:USERPROFILE ".templateengine"

# If no explicit version is given, generate a UNIQUE version every run.
# This is important: Visual Studio caches template content by (identity + version),
# so reinstalling the same version is ignored by VS even after a restart. A fresh
# version on every pack forces VS to re-extract the new template content.
if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = "1.0." + [int](Get-Date -Format "MMddHHmm")
    Write-Host "No -Version supplied; using auto-generated version $Version" -ForegroundColor DarkGray
}

# --- 0. Warn if Visual Studio is running ------------------------------------------
$vsRunning = Get-Process -Name "devenv" -ErrorAction SilentlyContinue
if ($vsRunning) {
    Write-Host "WARNING: Visual Studio is currently running." -ForegroundColor Yellow
    Write-Host "         You will need to close and reopen it for the template changes to appear." -ForegroundColor Yellow
}

# --- 0b. Clear Visual Studio's own template caches (only when VS is closed) --------
# VS keeps its OWN copy of installed templates (separate from the dotnet template
# engine). If these are not cleared, VS may keep showing/generating a stale template.
if (-not $vsRunning -and -not $SkipCacheReset) {
    $vsRoot = Join-Path $env:LOCALAPPDATA "Microsoft\VisualStudio"
    if (Test-Path $vsRoot) {
        Write-Host "Clearing Visual Studio template caches..." -ForegroundColor Cyan
        Get-ChildItem $vsRoot -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -match '^1[78]\.' } |   # VS 2022 (17.x) and VS 2026 (18.x)
            ForEach-Object {
                Get-ChildItem $_.FullName -Directory -ErrorAction SilentlyContinue |
                    Where-Object { $_.Name -match 'TemplateEngineHost|ProjectTemplatesCache|ItemTemplatesCache|^STemplate$' } |
                    ForEach-Object {
                        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
                    }
            }
    }
}
elseif ($vsRunning) {
    Write-Host "Skipping Visual Studio cache cleanup because VS is running (close it and re-run for a full reset)." -ForegroundColor Yellow
}

# --- 1. Uninstall any previous version (ignore errors if not installed) ------------
Write-Host "Removing any previously installed version of the template..." -ForegroundColor Cyan
& {
    $ErrorActionPreference = "SilentlyContinue"
    dotnet new uninstall $root      *> $null
    dotnet new uninstall $packageId *> $null
}

# --- 1b. Clear the template engine cache ------------------------------------------
if (-not $SkipCacheReset) {
    if ($HardReset) {
        Write-Host "Hard reset: deleting the entire template engine cache ($teDir)..." -ForegroundColor Cyan
        if (Test-Path $teDir) {
            Remove-Item $teDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
    else {
        Write-Host "Clearing the template engine cache..." -ForegroundColor Cyan
        if (Test-Path $teDir) {
            # Remove the cache/settings files but keep packages.json (the list of
            # installed template packages) so other installed templates survive.
            Get-ChildItem -Path $teDir -Recurse -File -ErrorAction SilentlyContinue |
                Where-Object { $_.Name -ne "packages.json" } |
                Remove-Item -Force -ErrorAction SilentlyContinue
        }
    }
}

# --- 2. Pack -----------------------------------------------------------------------
Write-Host "Packing the template into a local NuGet package..." -ForegroundColor Cyan

$packArgs = @("pack", $packProj, "-c", "Release", "-o", $nupkgDir)
if (-not [string]::IsNullOrWhiteSpace($Version)) {
    $packArgs += "/p:PackageVersion=$Version"
}

dotnet @packArgs
if ($LASTEXITCODE -ne 0) {
    Write-Host "Pack failed (exit code $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

# --- 3. Install the freshly built package ------------------------------------------
$pkg = Get-ChildItem -Path $nupkgDir -Filter "*.nupkg" |
       Sort-Object LastWriteTime -Descending |
       Select-Object -First 1

if ($null -eq $pkg) {
    Write-Host "Could not find the generated .nupkg in $nupkgDir." -ForegroundColor Red
    exit 1
}

Write-Host "Installing template package: $($pkg.Name)" -ForegroundColor Cyan
dotnet new install $pkg.FullName --force

if ($LASTEXITCODE -eq 0) {
    # --- 4. Rebuild the template engine cache so the new content is picked up ------
    if (-not $SkipCacheReset) {
        Write-Host "Rebuilding the template engine cache..." -ForegroundColor Cyan
        & {
            $ErrorActionPreference = "SilentlyContinue"
            dotnet new --debug:rebuildcache *> $null
        }
    }

    Write-Host ""
    Write-Host "Template installed from local package." -ForegroundColor Green
    Write-Host "IMPORTANT: Close and reopen Visual Studio so it reloads the template list." -ForegroundColor Yellow
    Write-Host "           (The template engine cache was cleared, but Visual Studio caches" -ForegroundColor Yellow
    Write-Host "            its templates in memory at startup and cannot be refreshed while open.)" -ForegroundColor Yellow
    Write-Host "CLI usage: dotnet new jackobo-webapi -n My.New.Service" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Install failed (exit code $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}
