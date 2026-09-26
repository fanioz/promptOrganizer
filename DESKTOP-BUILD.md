# PromptOrganizer Desktop Build Configuration

## Current Build Target
The project is currently configured to build **only for Desktop (Windows)** as the primary target for R1 release.

## Build Configuration Changes Made

### 1. Project File Updates
- **Main Project**: [`PromptOrganizer.App.csproj`](PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj)
  - Changed from multi-target: `net10.0-android;net10.0-ios;net10.0-browserwasm;net10.0-desktop`
  - To single target: `net10.0-desktop`

- **User Project**: [`PromptOrganizer.App.csproj.user`](PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj.user)
  - Updated TFM from `net10.0-android` to `net10.0-desktop`

### 2. Build Scripts Created
- **Windows Batch**: [`build-desktop.bat`](build-desktop.bat) - For Windows Command Line
- **PowerShell**: [`build-desktop.ps1`](build-desktop.ps1) - Cross-platform PowerShell

## How to Build for Desktop

### Option 1: Using Build Scripts
```bash
# Windows Command Line
build-desktop.bat

# PowerShell (Windows/Linux/Mac)
powershell -ExecutionPolicy Bypass -File build-desktop.ps1
```

### Option 2: Direct dotnet command
```bash
dotnet build PromptOrganizer.App/PromptOrganizer.App/PromptOrganizer.App.csproj -f net10.0-desktop
```

### Option 3: From project directory
```bash
cd PromptOrganizer.App/PromptOrganizer.App
dotnet build -f net10.0-desktop
```

## Build Output
- **Location**: `PromptOrganizer.App\PromptOrganizer.App\bin\Debug\net10.0-desktop\`
- **Main Executable**: `PromptOrganizer.App.exe`
- **Dependencies**: All required DLLs and runtime files

## Why Desktop-Only for R1?
- **Focus**: Concentrate development efforts on Windows desktop experience
- **Performance**: Optimize for desktop-specific features and performance
- **Stability**: Reduce complexity by targeting single platform initially
- **User Base**: Primary target audience uses Windows desktop

## Future Platform Support
Other platforms (Android, iOS, WebAssembly) are planned for future releases:
- **R2**: Mobile companion apps (Android/iOS) with cloud sync
- **R3**: Web viewer for read-only access

## Technical Details
- **Framework**: .NET 10.0 with Uno Platform
- **Runtime**: Desktop Skia renderer
- **Database**: LiteDB for local encrypted storage
- **UI**: XAML with WinUI controls
- **Architecture**: Clean Architecture with MVVM pattern

## Development Notes
- Build warnings about nullable events are known and will be addressed
- Hot reload is enabled for development
- SkiaRenderer provides consistent cross-platform rendering
- All class library projects remain platform-agnostic (`net10.0`)