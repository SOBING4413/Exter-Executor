using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Logging;
using ExterExecutor.App.Core.Services;
using ExterExecutor.App.Features.Dashboard;
using ExterExecutor.App.Features.Editor;
using ExterExecutor.App.Features.Scripts;
using ExterExecutor.App.Theme;

namespace ExterExecutor.App.UI;

internal sealed class MainShellForm : Form
{
    private readonly Panel _contentPanel;
    private readonly Label _notificationLabel;
    private readonly System.Windows.Forms.Timer _notificationTimer;
    private readonly Dictionary<string, Control> _views;

    public MainShellForm(AppSettingsProvider settingsProvider, IAppLogger logger, NotificationService notificationService)
    {
        Text = settingsProvider.Settings.AppTitle;
        MinimumSize = new Size(1000, 620);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5F);
        BackColor = ColorPalette.AppBackground;
        ForeColor = ColorPalette.Foreground;

        var sidebar = BuildSidebar();
        _contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = ColorPalette.AppBackground };
        var topBar = BuildTopBar(settingsProvider.Settings.AppTitle);
        _notificationLabel = BuildNotificationHost();

        Controls.Add(_contentPanel);
        Controls.Add(_notificationLabel);
        Controls.Add(topBar);
        Controls.Add(sidebar);

        _views = new Dictionary<string, Control>
        {
            ["Dashboard"] = new DashboardView(),
            ["Editor"] = new ScriptEditorView(settingsProvider.Settings, notificationService),
            ["Scripts"] = new ScriptLibraryView(notificationService)
        };

        NavigateTo("Dashboard", logger);

        notificationService.NotificationRaised += message =>
        {
            _notificationLabel.Text = message;
            _notificationLabel.Visible = true;
            _notificationTimer.Stop();
            _notificationTimer.Start();
            logger.Info($"Notification displayed: {message}");
        };

        _notificationTimer = new System.Windows.Forms.Timer { Interval = 2200 };
        _notificationTimer.Tick += (_, _) =>
        {
            _notificationLabel.Visible = false;
            _notificationTimer.Stop();
        };
    }

    private Panel BuildSidebar()
    {
        var sidebar = new Panel
        {
            Width = 220,
            Dock = DockStyle.Left,
            BackColor = ColorPalette.Sidebar,
            Padding = new Padding(10)
        };

        var title = new Label
        {
            Text = "EXTER EXECUTOR",
            Dock = DockStyle.Top,
            Height = 48,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = ColorPalette.Foreground,
            TextAlign = ContentAlignment.MiddleCenter
        };

        sidebar.Controls.Add(CreateNavigationButton("Settings", () => MessageBox.Show("Settings panel coming soon.", "Info")));
        sidebar.Controls.Add(CreateNavigationButton("Scripts", () => NavigateTo("Scripts", null)));
        sidebar.Controls.Add(CreateNavigationButton("Editor", () => NavigateTo("Editor", null)));
        sidebar.Controls.Add(CreateNavigationButton("Dashboard", () => NavigateTo("Dashboard", null)));
        sidebar.Controls.Add(title);

        return sidebar;
    }

    private static Panel BuildTopBar(string appTitle)
    {
        var panel = new Panel
        {
            Height = 52,
            Dock = DockStyle.Top,
            BackColor = ColorPalette.Surface,
            Padding = new Padding(18, 0, 18, 0)
        };

        var label = new Label
        {
            Text = $"{appTitle} • Modernized UI",
            Dock = DockStyle.Left,
            Width = 320,
            ForeColor = ColorPalette.Foreground,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        panel.Controls.Add(label);
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
            ForeColor = ColorPalette.Foreground,
            Margin = new Padding(0, 8, 0, 0)
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);
        button.Click += (_, _) => onClick();
        return button;
    }

    private void NavigateTo(string key, IAppLogger? logger)
    {
        if (!_views.TryGetValue(key, out var view))
        {
            return;
        }

        _contentPanel.Controls.Clear();
        _contentPanel.Controls.Add(view);
        logger?.Info($"Navigated to {key}.");
    }
}
