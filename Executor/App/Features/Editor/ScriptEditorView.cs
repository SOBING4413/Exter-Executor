using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Editor;

internal sealed class ScriptEditorView : UserControl
{
    private readonly RichTextBox _editor;
    private readonly Button _injectButton;
    private readonly ComboBox _apiComboBox;
    private readonly ProgressBar _progressBar;
    private readonly ApiEndpointService _apiEndpointService;
    private readonly InjectionService _injectionService;
    private readonly NotificationService _notificationService;

    public ScriptEditorView(
        AppSettings settings,
        ApiEndpointService apiEndpointService,
        InjectionService injectionService,
        NotificationService notificationService)
    {
        _apiEndpointService = apiEndpointService;
        _injectionService = injectionService;
        _notificationService = notificationService;

        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(17, 24, 39);

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(12, 10, 12, 10), BackColor = Color.FromArgb(31, 41, 55) };

        _injectButton = CreateButton("Inject", InjectAsync);
        var clearButton = CreateButton("Clear", () => _editor.Clear());
        _apiComboBox = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
        _apiComboBox.SelectedValueChanged += (_, _) =>
        {
            if (_apiComboBox.SelectedValue is string apiId)
            {
                _apiEndpointService.Select(apiId);
                _notificationService.Show("Selected API endpoint updated.");
            }
        };

        _progressBar = new ProgressBar { Width = 120, Style = ProgressBarStyle.Marquee, Visible = false, MarqueeAnimationSpeed = 25 };

        toolbar.Controls.Add(_injectButton);
        toolbar.Controls.Add(clearButton);
        toolbar.Controls.Add(_apiComboBox);
        toolbar.Controls.Add(_progressBar);

        _editor = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.FromArgb(226, 232, 240),
            Font = new Font("Cascadia Code", settings.Editor.FontSize),
            WordWrap = settings.Editor.WordWrap,
            Text = "-- Ready\nprint('Executor initialized')"
        };

        Controls.Add(_editor);
        Controls.Add(toolbar);
        BindApis();
    }

    public void RefreshApiList() => BindApis();

    private async void InjectAsync()
    {
        _injectButton.Enabled = false;
        _progressBar.Visible = true;

        try
        {
            var result = await _injectionService.InjectAsync(_editor.Text, CancellationToken.None);
            _notificationService.Show(result.Message);
        }
        finally
        {
            _progressBar.Visible = false;
            _injectButton.Enabled = true;
        }
    }

    private void BindApis()
    {
        var endpoints = _apiEndpointService.GetAll().ToList();
        _apiComboBox.DataSource = endpoints;
        _apiComboBox.DisplayMember = nameof(ApiEndpointSettings.Name);
        _apiComboBox.ValueMember = nameof(ApiEndpointSettings.Id);
        _apiComboBox.SelectedValue = _apiEndpointService.GetSelected().Id;
    }

    private static Button CreateButton(string text, Action onClick)
    {
        var button = new Button { Text = text, Width = 120, Height = 30, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Margin = new Padding(0, 0, 10, 0) };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => onClick();
        return button;
    }
}
