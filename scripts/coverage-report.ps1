# Test Coverage Report Generation Script
# This script generates comprehensive test coverage reports using Coverlet and ReportGenerator

param(
    [string]$Configuration = "Debug",
    [string]$OutputPath = "./coverage",
    [string]$ReportType = "Html",
    [switch]$IncludeHistory = $false,
    [switch]$FailBelowThreshold = $false,
    [int]$LineCoverageThreshold = 80,
    [int]$BranchCoverageThreshold = 75,
    [int]$MethodCoverageThreshold = 80
)

Write-Host "Starting Test Coverage Report Generation..." -ForegroundColor Green

# Create output directory if it doesn't exist
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath | Out-Null
}

# Clean previous coverage results
Write-Host "Cleaning previous coverage results..." -ForegroundColor Yellow
Remove-Item -Path "$OutputPath/*" -Recurse -Force -ErrorAction SilentlyContinue

# Run tests with coverage collection
Write-Host "Running tests with coverage collection..." -ForegroundColor Yellow
dotnet test `
    --configuration $Configuration `
    --collect:"XPlat Code Coverage" `
    --results-directory:$OutputPath `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover `
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="[xunit.*]*" `
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute="Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute" `
    DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile="**/Migrations/**"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Test execution failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Find the coverage file
$coverageFiles = Get-ChildItem -Path $OutputPath -Recurse -Filter "coverage.cobertura.xml"
if ($coverageFiles.Count -eq 0) {
    Write-Host "No coverage files found!" -ForegroundColor Red
    exit 1
}

$coverageFile = $coverageFiles[0].FullName
Write-Host "Found coverage file: $coverageFile" -ForegroundColor Green

# Generate detailed coverage report
Write-Host "Generating detailed coverage report..." -ForegroundColor Yellow
$reportGeneratorArgs = @(
    "-reports:$coverageFile",
    "-targetdir:$OutputPath/report",
    "-reporttypes:$ReportType",
    "-sourcedirs:./src",
    "-historydir:$OutputPath/history",
    "-plugins:RiskHotspots",
    "-assemblyfilters:+PromptOrganizer.*",
    "-classfilters:-PromptOrganizer.Tests.*",
    "-filefilters:-*/Migrations/*"
)

if ($IncludeHistory) {
    $reportGeneratorArgs += "-historydir:$OutputPath/history"
}

dotnet reportgenerator $reportGeneratorArgs

# Generate summary report
Write-Host "Generating coverage summary..." -ForegroundColor Yellow
$summaryArgs = @(
    "-reports:$coverageFile",
    "-targetdir:$OutputPath/summary",
    "-reporttypes:TextSummary",
    "-sourcedirs:./src",
    "-assemblyfilters:+PromptOrganizer.*"
)

dotnet reportgenerator $summaryArgs

# Display coverage summary
Write-Host "`n=== COVERAGE SUMMARY ===" -ForegroundColor Cyan
Get-Content "$OutputPath/summary/Summary.txt" | Write-Host

# Check coverage thresholds
if ($FailBelowThreshold) {
    Write-Host "`nChecking coverage thresholds..." -ForegroundColor Yellow
    
    # Parse coverage data from XML
    [xml]$coverageXml = Get-Content $coverageFile
    $lineCoverage = [math]::Round([double]$coverageXml.coverage.'line-rate' * 100, 2)
    $branchCoverage = [math]::Round([double]$coverageXml.coverage.'branch-rate' * 100, 2)
    
    Write-Host "Line Coverage: $lineCoverage% (Threshold: $LineCoverageThreshold%)" -ForegroundColor $(if ($lineCoverage -ge $LineCoverageThreshold) { "Green" } else { "Red" })
    Write-Host "Branch Coverage: $branchCoverage% (Threshold: $BranchCoverageThreshold%)" -ForegroundColor $(if ($branchCoverage -ge $BranchCoverageThreshold) { "Green" } else { "Red" })
    
    $failedThresholds = @()
    if ($lineCoverage -lt $LineCoverageThreshold) { $failedThresholds += "Line Coverage" }
    if ($branchCoverage -lt $BranchCoverageThreshold) { $failedThresholds += "Branch Coverage" }
    
    if ($failedThresholds.Count -gt 0) {
        Write-Host "`nFAILED: Coverage thresholds not met for: $($failedThresholds -join ', ')" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "`nSUCCESS: All coverage thresholds met!" -ForegroundColor Green
    }
}

Write-Host "`nCoverage report generation completed!" -ForegroundColor Green
Write-Host "Full report available at: $(Resolve-Path "$OutputPath/report/index.html")" -ForegroundColor Cyan