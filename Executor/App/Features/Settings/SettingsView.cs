using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Settings;

internal sealed class SettingsView : UserControl
{
    private readonly AppSettingsProvider _settingsProvider;
    private readonly ApiEndpointService _apiEndpointService;
    private readonly NotificationService _notificationService;
    private readonly CheckBox _topMost;
    private readonly CheckBox _minimap;
    private readonly CheckBox _disableCloseText;
    private readonly FlowLayoutPanel _apiRadioPanel;

    public event Action? SettingsUpdated;

    public SettingsView(AppSettingsProvider settingsProvider, ApiEndpointService apiEndpointService, NotificationService notificationService)
    {
        _settingsProvider = settingsProvider;
        _apiEndpointService = apiEndpointService;
        _notificationService = notificationService;

        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(7, 10, 22);

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(20),
            AutoScroll = true
        };

        _topMost = CreateToggleRow(layout, "Top Most", settingsProvider.Settings.TopMost);
        _minimap = CreateToggleRow(layout, "Toggle Minimap Scrollbar", settingsProvider.Settings.ToggleMinimapScrollbar);
        _disableCloseText = CreateToggleRow(layout, "Disable Close Tab Text", settingsProvider.Settings.DisableCloseTabText);

        var apiContainer = new Panel
        {
            Width = 680,
            Height = 88,
            BackColor = Color.FromArgb(12, 16, 37),
            Padding = new Padding(12),
            Margin = new Padding(0, 8, 0, 8)
        };
        apiContainer.Controls.Add(new Label { Text = "API:", ForeColor = Color.White, Width = 80, Dock = DockStyle.Left, Font = new Font("Segoe UI", 11, FontStyle.Regular) });

        _apiRadioPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(8, 12, 0, 0) };
        apiContainer.Controls.Add(_apiRadioPanel);

        var saveButton = new Button
        {
            Text = "Save Settings",
            Width = 180,
            Height = 36,
            BackColor = Color.FromArgb(122, 85, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += (_, _) => SaveSettings();

        layout.Controls.Add(apiContainer);
        layout.Controls.Add(saveButton);
        Controls.Add(layout);

        PopulateApiSelectors();
    }

    private CheckBox CreateToggleRow(Control parent, string title, bool value)
    {
        var row = new Panel
        {
            Width = 680,
            Height = 54,
            BackColor = Color.FromArgb(12, 16, 37),
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 8)
        };

        var label = new Label { Text = title, ForeColor = Color.White, Dock = DockStyle.Left, Width = 340, Font = new Font("Segoe UI", 10.5F) };
        var toggle = new CheckBox { Checked = value, Dock = DockStyle.Right, Width = 40, Height = 24 };

        row.Controls.Add(toggle);
        row.Controls.Add(label);
        parent.Controls.Add(row);
        return toggle;
    }

    private void PopulateApiSelectors()
    {
        _apiRadioPanel.Controls.Clear();
        var activeId = _apiEndpointService.GetSelected().Id;

        foreach (var endpoint in _apiEndpointService.GetAll().Where(api => api.IsEnabled))
        {
            var radio = new RadioButton
            {
                Text = endpoint.Name,
                ForeColor = Color.White,
                Width = 150,
                Checked = endpoint.Id == activeId,
                Tag = endpoint.Id
            };
            radio.CheckedChanged += (_, _) =>
            {
                if (radio.Checked && radio.Tag is string apiId)
                {
                    _apiEndpointService.Select(apiId);
                    _notificationService.Show($"API switched to {endpoint.Name}.");
                }
            };
            _apiRadioPanel.Controls.Add(radio);
        }
    }

    private void SaveSettings()
    {
        _settingsProvider.Settings.TopMost = _topMost.Checked;
        _settingsProvider.Settings.ToggleMinimapScrollbar = _minimap.Checked;
        _settingsProvider.Settings.DisableCloseTabText = _disableCloseText.Checked;
        _settingsProvider.Save();
        _notificationService.Show("Settings saved.");
        SettingsUpdated?.Invoke();
    }
}
