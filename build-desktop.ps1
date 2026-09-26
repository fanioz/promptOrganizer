# PromptOrganizer Desktop Build Script
# Cross-platform PowerShell script for building desktop target

Write-Host "Building PromptOrganizer for Desktop (Windows)" -ForegroundColor Green
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj

# Build for desktop target
Write-Host "Building for desktop target..." -ForegroundColor Yellow
dotnet build PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj -f net10.0-desktop -c Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Desktop build completed successfully!" -ForegroundColor Green
    Write-Host "Output location: PromptOrganizer.App\PromptOrganizer.App\bin\Debug\net10.0-desktop\" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}