using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Settings;

internal sealed class SettingsView : UserControl
{
    private readonly AppSettingsProvider _settingsProvider;
    private readonly ApiEndpointService _apiEndpointService;
    private readonly NotificationService _notificationService;

    // General settings
    private readonly TextBox _username;
    private readonly ComboBox _theme;
    private readonly CheckBox _wordWrap;
    private readonly NumericUpDown _fontSize;
    private readonly CheckBox _autoReconnect;
    private readonly CheckBox _topMost;
    private readonly CheckBox _minimap;
    private readonly CheckBox _disableCloseText;

    // API settings
    private readonly ListBox _apiList;
    private readonly TextBox _apiName;
    private readonly TextBox _apiUrl;

    public event Action? SettingsUpdated;

    public SettingsView(AppSettingsProvider settingsProvider, ApiEndpointService apiEndpointService, NotificationService notificationService)
    {
        _settingsProvider = settingsProvider;
        _apiEndpointService = apiEndpointService;
        _notificationService = notificationService;

        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(17, 24, 39);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(24) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        // General Section
        var general = CreateSection("General");

        _username = CreateTextBox(settingsProvider.Settings.Username);
        _theme = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        _theme.Items.AddRange(new[] { "Dark", "Light" });
        _theme.SelectedItem = settingsProvider.Settings.Theme;

        _wordWrap = new CheckBox { Text = "Word Wrap", ForeColor = Color.White, Checked = settingsProvider.Settings.Editor.WordWrap };
        _fontSize = new NumericUpDown { Minimum = 8, Maximum = 24, Value = settingsProvider.Settings.Editor.FontSize };
        _autoReconnect = new CheckBox { Text = "Discord Auto Reconnect", ForeColor = Color.White, Checked = settingsProvider.Settings.AutoReconnectDiscord };

        // Legacy toggles
        _topMost = new CheckBox { Text = "Top Most", ForeColor = Color.White, Checked = settingsProvider.Settings.TopMost };
        _minimap = new CheckBox { Text = "Toggle Minimap Scrollbar", ForeColor = Color.White, Checked = settingsProvider.Settings.ToggleMinimapScrollbar };
        _disableCloseText = new CheckBox { Text = "Disable Close Tab Text", ForeColor = Color.White, Checked = settingsProvider.Settings.DisableCloseTabText };

        AddRow(general, "Username", _username);
        AddRow(general, "Theme", _theme);
        AddRow(general, "Font Size", _fontSize);
        general.Controls.Add(_wordWrap);
        general.Controls.Add(_autoReconnect);
        general.Controls.Add(_topMost);
        general.Controls.Add(_minimap);
        general.Controls.Add(_disableCloseText);

        // API Section
        var apiSection = CreateSection("API Endpoints");
        _apiList = new ListBox { Width = 260, Height = 150, BackColor = Color.FromArgb(31, 41, 55), ForeColor = Color.White, BorderStyle = BorderStyle.None };
        _apiName = CreateTextBox(string.Empty);
        _apiUrl = CreateTextBox("https://");

        apiSection.Controls.Add(_apiList);
        AddRow(apiSection, "Name", _apiName);
        AddRow(apiSection, "URL", _apiUrl);

        var addButton = CreateButton("Add API", AddApi);
        var removeButton = CreateButton("Remove API", RemoveApi);
        var selectButton = CreateButton("Set Active API", SelectApi);
        var saveButton = CreateButton("Save Settings", SaveSettings);

        apiSection.Controls.Add(addButton);
        apiSection.Controls.Add(removeButton);
        apiSection.Controls.Add(selectButton);
        apiSection.Controls.Add(saveButton);

        root.Controls.Add(general, 0, 0);
        root.Controls.Add(apiSection, 1, 0);
        Controls.Add(root);

        RefreshApiList();
    }

    private void SaveSettings()
    {
        _settingsProvider.Settings.Username = _username.Text.Trim();
        _settingsProvider.Settings.Theme = (_theme.SelectedItem?.ToString()) ?? "Dark";
        _settingsProvider.Settings.Editor.FontSize = (int)_fontSize.Value;
        _settingsProvider.Settings.Editor.WordWrap = _wordWrap.Checked;
        _settingsProvider.Settings.AutoReconnectDiscord = _autoReconnect.Checked;

        // Legacy toggles
        _settingsProvider.Settings.TopMost = _topMost.Checked;
        _settingsProvider.Settings.ToggleMinimapScrollbar = _minimap.Checked;
        _settingsProvider.Settings.DisableCloseTabText = _disableCloseText.Checked;

        _settingsProvider.Save();
        _notificationService.Show("Settings saved.");
        SettingsUpdated?.Invoke();
    }

    private void AddApi()
    {
        try
        {
            _apiEndpointService.Add(_apiName.Text, _apiUrl.Text);
            RefreshApiList();
            _notificationService.Show("API endpoint added.");
        }
        catch (Exception ex)
        {
            _notificationService.Show(ex.Message);
        }
    }

    private void RemoveApi()
    {
        try
        {
            if (_apiList.SelectedItem is ApiEndpointSettings endpoint)
            {
                _apiEndpointService.Remove(endpoint.Id);
                RefreshApiList();
                _notificationService.Show("API endpoint removed.");
            }
        }
        catch (Exception ex)
        {
            _notificationService.Show(ex.Message);
        }
    }

    private void SelectApi()
    {
        if (_apiList.SelectedItem is not ApiEndpointSettings endpoint) return;

        _apiEndpointService.Select(endpoint.Id);
        RefreshApiList();
        _notificationService.Show($"Active API set to '{endpoint.Name}'.");
        SettingsUpdated?.Invoke();
    }

    private void RefreshApiList()
    {
        _apiList.DataSource = null;
        _apiList.DataSource = _apiEndpointService.GetAll().ToList();
        _apiList.DisplayMember = nameof(ApiEndpointSettings.Name);
        _apiList.SelectedItem = _apiEndpointService.GetSelected();
    }

    private static FlowLayoutPanel CreateSection(string title)
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(16),
            BackColor = Color.FromArgb(31, 41, 55)
        };

        panel.Controls.Add(new Label { Text = title, ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), Width = 280, Height = 30 });
        return panel;
    }

    private static void AddRow(Control section, string label, Control input)
    {
        section.Controls.Add(new Label { Text = label, ForeColor = Color.FromArgb(148, 163, 184), Width = 280, Height = 20 });
        input.Width = 280;
        input.Height = 28;
        section.Controls.Add(input);
    }

    private static TextBox CreateTextBox(string value) => new() { Text = value, BackColor = Color.FromArgb(15, 23, 42), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

    private static Button CreateButton(string text, Action onClick)
    {
        var button = new Button { Text = text, Width = 180, Height = 32, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => onClick();
        return button;
    }
}