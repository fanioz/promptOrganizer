@echo off
echo Building PromptOrganizer for Desktop (Windows)...
echo.

REM Clean previous builds
echo Cleaning previous builds...
dotnet clean PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj

REM Restore dependencies
echo Restoring dependencies...
dotnet restore PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj

REM Build for desktop target
echo Building for desktop target...
dotnet build PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj -f net10.0-desktop -c Debug

echo.
echo Desktop build completed!
echo Output location: PromptOrganizer.App\PromptOrganizer.App\bin\Debug\net10.0-desktop\
echo.
pause