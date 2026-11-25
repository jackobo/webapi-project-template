<#
.SYNOPSIS
    Installs (or reinstalls) the Jackobo Web API project template from this folder.

.DESCRIPTION
    Registers the template located in the same folder as this script with the
    .NET CLI, so it becomes available via `dotnet new jackobo-webapi` and in the
    Visual Studio "Create a new project" dialog.

.EXAMPLE
    .\install-template.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

# The template lives in the same folder as this script.
$templateRoot = $PSScriptRoot

Write-Host "Installing the Jackobo Web API template from:" -ForegroundColor Cyan
Write-Host "  $templateRoot"
Write-Host ""

dotnet new install $templateRoot --force

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Template installed successfully." -ForegroundColor Green
    Write-Host "Create a new project with: dotnet new jackobo-webapi -n My.New.Service" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Template installation failed (exit code $LASTEXITCODE)." -ForegroundColor Red
}
