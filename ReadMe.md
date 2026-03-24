<div align="center">

# ⚡ Exter Executor

### A Modern, Feature-Rich Roblox Script Executor

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows)](https://github.com/SOBING4413/Exter-Executor)
[![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.7.2+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

<br/>

**Exter Executor** is a sleek and powerful Windows desktop application built with C# WinForms, designed for executing Roblox scripts with an intuitive, modern interface. Featuring an integrated Monaco code editor, a built-in Script Hub, multi-API support, and a secure key-based authentication system.

<br/>

---

</div>

## 📋 Table of Contents

- [✨ Features](#-features)
- [📸 Screenshots](#-screenshots)
- [🛠️ Tech Stack](#️-tech-stack)
- [📦 Prerequisites](#-prerequisites)
- [🚀 Getting Started](#-getting-started)
- [📂 Project Structure](#-project-structure)
- [⚙️ Configuration](#️-configuration)
- [🔑 Authentication](#-authentication)
- [📜 Script Hub](#-script-hub)
- [🎛️ Settings](#️-settings)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [⚠️ Disclaimer](#️-disclaimer)

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🔐 **Secure Login** | Key-based authentication system powered by KeyAuth API with license validation |
| 📝 **Monaco Editor** | Full-featured code editor with Lua syntax highlighting, IntelliSense, and multi-tab support |
| 📚 **Script Hub** | Browse and load pre-made scripts directly from a curated GitHub repository |
| 🔄 **Multi-API Support** | Choose between **QuorumAPI**, **AeigisAPI**, and **SpashAPI** for script execution |
| 📌 **Always on Top** | Pin the executor window above all other applications |
| 🗺️ **Minimap Toggle** | Enable/disable the code minimap in the Monaco editor |
| 💾 **Persistent Settings** | All preferences are saved locally and restored on next launch |
| 📰 **News & Changelog** | Stay updated with the latest news fetched directly from GitHub |
| 🎨 **Modern UI** | Beautiful, dark-themed interface built with Guna.UI2 components |
| 📁 **File Operations** | Open and save Lua script files directly from the editor |
| 🗂️ **Multi-Tab Editing** | Work on multiple scripts simultaneously with tabbed editing |

---

## 📸 Screenshots

> *Screenshots coming soon — contributions welcome!*

---

## 🛠️ Tech Stack

| Technology | Purpose |
|-----------|---------|
| **C# (.NET Framework)** | Core application language and runtime |
| **Windows Forms (WinForms)** | Desktop UI framework |
| **Guna.UI2** | Modern UI component library for WinForms |
| **Monaco Editor** | Embedded web-based code editor (via WebBrowser control) |
| **KeyAuth** | License/key authentication service |
| **Newtonsoft.Json** | JSON serialization and deserialization |
| **GitHub Raw Content** | Remote script and news content delivery |

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- **Windows 10/11** (64-bit recommended)
- **Visual Studio 2019+** with the following workloads:
  - `.NET Desktop Development`
- **.NET Framework 4.7.2** or later
- **NuGet Package Manager** (included with Visual Studio)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/SOBING4413/Exter-Executor.git
cd Exter-Executor
```

### 2. Open the Solution

Open `Exter Executor.sln` in Visual Studio.

### 3. Restore NuGet Packages

Visual Studio should automatically restore packages. If not, right-click the solution in **Solution Explorer** → **Restore NuGet Packages**.

Required packages:
- `Guna.UI2.WinForms`
- `Newtonsoft.Json`
- `KeyAuth`

### 4. Build the Project

```
Build → Build Solution (Ctrl + Shift + B)
```

### 5. Run

Press `F5` or click **Start** to launch the application.

---

## 📂 Project Structure

```
Exter-Executor/
├── Executor/                    # Main application project
│   ├── Monaco/                  # Monaco Editor web assets
│   │   └── index.html           # Monaco Editor HTML interface
│   ├── Properties/              # Assembly info & resources
│   ├── Resources/               # Embedded resources (icons, images)
│   ├── bin/                     # Build output
│   ├── obj/                     # Intermediate build files
│   │
│   ├── Form1.cs                 # Main form (navigation & layout)
│   ├── Form1.Designer.cs        # Main form designer
│   ├── Login.cs                 # Login/authentication form
│   ├── Login.Designer.cs        # Login form designer
│   ├── Executor.cs              # Script executor (Monaco editor + tabs)
│   ├── Executor.Designer.cs     # Executor form designer
│   ├── Home.cs                  # Home page (news & changelog)
│   ├── ScriptHub.cs             # Script Hub browser
│   ├── ScriptHub.Designer.cs    # Script Hub designer
│   ├── Settings.cs              # Settings panel
│   ├── Settings.Designer.cs     # Settings panel designer
│   ├── Program.cs               # Application entry point
│   ├── App.config               # Application configuration
│   ├── app.manifest             # Application manifest
│   ├── packages.config          # NuGet package references
│   └── Exter Executor.csproj    # Project file
│
├── packages/                    # NuGet packages (auto-restored)
├── Exter Executor.sln           # Visual Studio solution file
├── LICENSE                      # MIT License
└── ReadMe.txt                   # Original readme
```

---

## ⚙️ Configuration

### Application Settings

Settings are persisted locally via `AppSettingsManager` and include:

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `IsTopmost` | `bool` | `false` | Keep window always on top |
| `DisableConfirm` | `bool` | `false` | Disable close-tab confirmation dialog |
| `MinimapToggle` | `bool` | `false` | Show/hide Monaco editor minimap |
| `ChosenApi` | `string` | `QuorumAPI` | Selected execution API |

### Modifying KeyAuth Configuration

The authentication system is configured in `Login.cs`. To use your own KeyAuth application:

```csharp
public static api KeyAuthApp = new api(
    name: "YourAppName",
    ownerid: "YourOwnerID",
    secret: "YourSecret",
    version: "1.0"
);
```

> ⚠️ **Note:** You must have a valid [KeyAuth](https://keyauth.cc/) account and application configured.

---

## 🔑 Authentication

Exter Executor uses **KeyAuth** for license-based authentication:

1. Launch the application → the **Login** screen appears
2. Enter your **license key** in the input field
3. Click **Login** to validate your key
4. Upon successful authentication, you are redirected to the **Home** page

The login system supports:
- ✅ License key validation
- ✅ Session management
- ✅ Automatic initialization check
- ✅ Error handling with user-friendly messages

---

## 📜 Script Hub

The **Script Hub** provides a curated collection of pre-made scripts:

- Scripts are fetched from a remote GitHub repository
- Each script displays its **name** and **game compatibility**
- Click **Copy** to copy the script content to your clipboard
- Click **Load** to load the script directly into the Monaco editor
- Scripts are loaded dynamically and rendered in a scrollable panel

---

## 🎛️ Settings

The Settings panel allows you to customize your experience:

| Option | Description |
|--------|-------------|
| **TopMost** | Toggle to keep the executor window above all other windows |
| **Disable Close Tab Message** | Skip the confirmation dialog when closing editor tabs |
| **Minimap** | Toggle the code minimap in the Monaco editor |
| **API Selection** | Choose your preferred execution API: QuorumAPI, AeigisAPI, or SpashAPI |

All settings are automatically saved and restored between sessions.

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork** the repository
2. **Create** a feature branch:
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Commit** your changes:
   ```bash
   git commit -m "feat: add amazing feature"
   ```
4. **Push** to the branch:
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Open** a Pull Request

### Contribution Guidelines

- Follow existing code style and naming conventions
- Test your changes thoroughly before submitting
- Write clear, descriptive commit messages
- Update documentation if needed

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2025 SOBING4413

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

---

## ⚠️ Disclaimer

> **This project is provided for educational and research purposes only.** The developers are not responsible for any misuse of this software. Use at your own risk and ensure compliance with all applicable terms of service and laws. This tool is not affiliated with or endorsed by Roblox Corporation.

---

<div align="center">

**Made with ❤️ by [SOBING4413](https://github.com/SOBING4413)**

⭐ **Star this repo if you find it useful!** ⭐

</div>
