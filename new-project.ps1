<#
.SYNOPSIS
    Creates a new project from the Jackobo Web API template.

.DESCRIPTION
    Wraps `dotnet new jackobo-webapi`. If the template is not installed yet, run
    install-template.ps1 first. The project name is used as the root namespace
    and solution name (e.g. MyCompany.Orders.Service).

.PARAMETER Name
    The name of the new project / solution / root namespace.

.PARAMETER AppTitle
    The display title of the service (Swagger document title), e.g. "Orders API".

.PARAMETER ElasticIndex
    The elastic index identifying the service in the logs (Logger:elasticIndex),
    e.g. "orders-service".

.PARAMETER RedisKeyPrefix
    The prefix namespacing every key this service writes in Redis (Redis:KeyPrefix),
    e.g. "OrdersService". Declared by every service, whether it uses Redis or not.

.PARAMETER ApiErrorsPrefix
    The short code prepended to every error code in Core/Errors/ApiErrors.cs,
    e.g. "ORD" turns "Database.RecordNotFound" into "ORD.Database.RecordNotFound".

.PARAMETER Output
    Optional output directory. Defaults to a subfolder named after the project
    in the current directory.

.EXAMPLE
    .\new-project.ps1 -Name MyCompany.Orders.Service `
                      -AppTitle "Orders API" `
                      -ElasticIndex orders-service `
                      -RedisKeyPrefix OrdersService `
                      -ApiErrorsPrefix ORD

.EXAMPLE
    .\new-project.ps1 -Name MyCompany.Orders.Service `
                      -AppTitle "Orders API" `
                      -ElasticIndex orders-service `
                      -RedisKeyPrefix OrdersService `
                      -ApiErrorsPrefix ORD `
                      -Output C:\Projects\orders-service
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [string]$AppTitle,

    [Parameter(Mandatory = $true)]
    [string]$ElasticIndex,

    [Parameter(Mandatory = $true)]
    [string]$RedisKeyPrefix,

    [Parameter(Mandatory = $true)]
    [string]$ApiErrorsPrefix,

    [Parameter(Mandatory = $false)]
    [string]$Output
)

$ErrorActionPreference = "Stop"

Write-Host "Creating a new Jackobo Web API project: $Name" -ForegroundColor Cyan

$templateArgs = @(
    "new", "jackobo-webapi",
    "-n", $Name,
    "--AppTitle", $AppTitle,
    "--ElasticIndex", $ElasticIndex,
    "--RedisKeyPrefix", $RedisKeyPrefix,
    "--ApiErrorsPrefix", $ApiErrorsPrefix
)

if (-not [string]::IsNullOrWhiteSpace($Output)) {
    $templateArgs += @("-o", $Output)
}

dotnet @templateArgs

if ($LASTEXITCODE -eq 0) {
    $location = if ([string]::IsNullOrWhiteSpace($Output)) { $Name } else { $Output }
    Write-Host ""
    Write-Host "Project created successfully in: $location" -ForegroundColor Green
    Write-Host "Next steps:" -ForegroundColor Green
    Write-Host "  cd `"$location`""
    Write-Host "  dotnet build"
    Write-Host "  dotnet run --project WebAPI"
} else {
    Write-Host ""
    Write-Host "Project creation failed (exit code $LASTEXITCODE)." -ForegroundColor Red
    Write-Host "If the template is not installed, run install-template.ps1 first." -ForegroundColor Yellow
}
