# Exter Executor (Modernized)

Exter Executor has been fully re-architected into a modular, maintainable Windows Forms application targeting **.NET 8**.

## Highlights

- ✅ New layered architecture (`Boot`, `Core`, `Features`, `Theme`, `UI`)
- ✅ Dark, dashboard-style professional UI
- ✅ Sidebar navigation and responsive content area
- ✅ Modular feature views (Dashboard, Editor, Script Library)
- ✅ Centralized configuration via `appsettings.json`
- ✅ Structured file logging and global exception handling
- ✅ Cleaner naming, consistent formatting, and improved maintainability

## Project Structure

```text
Executor/
├─ App/
│  ├─ Boot/                  # Startup orchestration
│  ├─ Core/
│  │  ├─ Configuration/      # App settings models + provider
│  │  ├─ Logging/            # Logger abstractions and file logger
│  │  └─ Services/           # Cross-feature services (notifications)
│  ├─ Features/
│  │  ├─ Dashboard/          # Dashboard view
│  │  ├─ Editor/             # Script editor view
│  │  └─ Scripts/            # Script library view
│  ├─ Theme/                 # Shared UI palette
│  ├─ UI/                    # Main shell form and navigation
│  └─ Program.cs             # Entry point
├─ Exter Executor.csproj     # SDK-style .NET 8 WinForms project
└─ appsettings.json          # Runtime configuration
```

## Requirements

- Windows 10/11
- .NET SDK 8.0+

## Setup

```bash
dotnet restore "Executor/Exter Executor.csproj"
dotnet build "Executor/Exter Executor.csproj" -c Release
```

## Run

```bash
dotnet run --project "Executor/Exter Executor.csproj"
```

## Configuration

Edit `Executor/appsettings.json`:

- `appTitle`: Window/app title
- `logging.logFilePath`: Output log file path
- `editor.fontSize`: Editor font size
- `editor.wordWrap`: Toggle wrapping

## Usage

1. Launch the app.
2. Use the **left sidebar** to switch between Dashboard, Editor, and Scripts.
3. Use **Editor** actions (`Execute`, `Clear`) for quick script operations.
4. Use **Scripts** view to browse and deploy script templates.
5. Watch for bottom toast-style notifications and review logs in `logs/`.

## Notes

- The modernized codebase is intentionally structured for future expansion (settings panel, real script runtime integration, API clients, plugin system).
- Current feature modules are decoupled so each can evolve independently.
