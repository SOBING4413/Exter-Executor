using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Logging;
using ExterExecutor.App.Core.Services;
using ExterExecutor.App.Features.Dashboard;
using ExterExecutor.App.Features.Editor;
using ExterExecutor.App.Features.Scripts;
using ExterExecutor.App.Features.Settings;
using ExterExecutor.App.Theme;

namespace ExterExecutor.App.UI;

internal sealed class MainShellForm : Form
{
    private readonly Panel _contentPanel;
    private readonly Label _notificationLabel;
    private readonly Label _statusLabel;
    private readonly Label _userLabel;
    private readonly Label _apiLabel;
    private readonly System.Windows.Forms.Timer _notificationTimer;
    private readonly Dictionary<string, Control> _views;

    public MainShellForm(
        AppSettingsProvider settingsProvider,
        IAppLogger logger,
        NotificationService notificationService,
        AppStateService appStateService,
        ApiEndpointService apiEndpointService,
        InjectionService injectionService)
    {
        Text = settingsProvider.Settings.AppTitle;
        MinimumSize = new Size(1140, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5F);
        BackColor = ColorPalette.AppBackground;
        ForeColor = ColorPalette.Foreground;
        TopMost = settingsProvider.Settings.TopMost;

        _contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = ColorPalette.AppBackground };
        _notificationLabel = BuildNotificationHost();
        _notificationTimer = new System.Windows.Forms.Timer { Interval = 2500 };
        _statusLabel = new Label { Width = 160, ForeColor = Color.White, TextAlign = ContentAlignment.MiddleRight };
        _userLabel = new Label { Width = 240, ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft };
        _apiLabel = new Label { Width = 220, ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft };

        var settingsView = new SettingsView(settingsProvider, apiEndpointService, notificationService);
        var editorView = new ScriptEditorView(settingsProvider.Settings, apiEndpointService, injectionService, notificationService);

        _views = new Dictionary<string, Control>
        {
            ["Dashboard"] = new DashboardView(settingsProvider.Settings, appStateService),
            ["Editor"] = editorView,
            ["Scripts"] = new ScriptLibraryView(notificationService),
            ["Settings"] = settingsView
        };

        // Merge fix: update TopMost + header + editor refresh
        settingsView.SettingsUpdated += () =>
        {
            TopMost = settingsProvider.Settings.TopMost;
            UpdateHeader(settingsProvider.Settings, appStateService.Status, apiEndpointService.GetSelected().Name);
            editorView.RefreshApiList();
        };

        Controls.Add(_contentPanel);
        Controls.Add(_notificationLabel);
        Controls.Add(BuildTopBar(settingsProvider.Settings));
        Controls.Add(BuildSidebar());

        NavigateTo("Dashboard", logger);
        UpdateHeader(settingsProvider.Settings, appStateService.Status, apiEndpointService.GetSelected().Name);

        notificationService.NotificationRaised += message =>
        {
            _notificationLabel.Text = message;
            _notificationLabel.Visible = true;
            _notificationTimer.Stop();
            _notificationTimer.Start();
            logger.Info($"Notification displayed: {message}");
        };

        _notificationTimer.Tick += (_, _) =>
        {
            _notificationLabel.Visible = false;
            _notificationTimer.Stop();
        };

        appStateService.StatusChanged += status => UpdateHeader(settingsProvider.Settings, status, apiEndpointService.GetSelected().Name);
    }

    private Panel BuildSidebar()
    {
        var sidebar = new Panel { Width = 220, Dock = DockStyle.Left, BackColor = ColorPalette.Sidebar, Padding = new Padding(10) };
        sidebar.Controls.Add(CreateNavigationButton("Settings", () => NavigateTo("Settings", null)));
        sidebar.Controls.Add(CreateNavigationButton("Scripts", () => NavigateTo("Scripts", null)));
        sidebar.Controls.Add(CreateNavigationButton("Editor", () => NavigateTo("Editor", null)));
        sidebar.Controls.Add(CreateNavigationButton("Dashboard", () => NavigateTo("Dashboard", null)));
        sidebar.Controls.Add(new Label
        {
            Text = "EXTER EXECUTOR",
            Dock = DockStyle.Top,
            Height = 50,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        });
        return sidebar;
    }

    private Panel BuildTopBar(AppSettings settings)
    {
        var panel = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = ColorPalette.Surface, Padding = new Padding(16, 0, 16, 0) };
        var left = new Label
        {
            Text = $"{settings.AppTitle} v{Application.ProductVersion}",
            Dock = DockStyle.Left,
            Width = 270,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _userLabel.Dock = DockStyle.Left;
        _apiLabel.Dock = DockStyle.Left;
        _statusLabel.Dock = DockStyle.Right;
        panel.Controls.Add(_statusLabel);
        panel.Controls.Add(_apiLabel);
        panel.Controls.Add(_userLabel);
        panel.Controls.Add(left);
        return panel;
    }

    private Label BuildNotificationHost() => new()
    {
        Dock = DockStyle.Bottom,
        Height = 32,
        Visible = false,
        TextAlign = ContentAlignment.MiddleCenter,
        BackColor = Color.FromArgb(30, 64, 175),
        ForeColor = Color.White,
        Font = new Font("Segoe UI", 9, FontStyle.Bold)
    };

    private Button CreateNavigationButton(string text, Action onClick)
    {
        var button = new Button
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = ColorPalette.Sidebar,
            ForeColor = Color.White,
            Margin = new Padding(0, 8, 0, 0)
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);
        button.Click += (_, _) => onClick();
        return button;
    }

    private void UpdateHeader(AppSettings settings, RuntimeStatus status, string apiName)
    {
        _userLabel.Text = $"User: {settings.Username}";
        _apiLabel.Text = $"API: {apiName}";
        _statusLabel.Text = $"Status: {status}";
    }

    private void NavigateTo(string key, IAppLogger? logger)
    {
        if (!_views.TryGetValue(key, out var view)) return;

        _contentPanel.Controls.Clear();
        _contentPanel.Controls.Add(view);
        logger?.Info($"Navigated to {key}.");
    }
}