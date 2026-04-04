using System.Diagnostics;
using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Editor;

internal sealed class ScriptEditorView : UserControl
{
    private readonly RichTextBox _editor;
    private readonly Button _injectButton;
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
        BackColor = Color.FromArgb(4, 8, 20);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(12) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 88));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 12));

        _editor = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = Color.Black,
            ForeColor = Color.White,
            Font = new Font("Cascadia Code", settings.Editor.FontSize),
            WordWrap = settings.Editor.WordWrap,
            Text = "loadstring(game:HttpGet(\"https://example.com/script.lua\"))()"
        };

        var scriptPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 14, 32), Padding = new Padding(8) };
        var search = new TextBox { Dock = DockStyle.Top, PlaceholderText = "Search...", BackColor = Color.FromArgb(3, 7, 21), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        var list = new ListBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(7, 11, 28), ForeColor = Color.White };
        list.Items.AddRange(["AimBot.lua", "Animation Menu.lua", "Sambung Kata.lua", "UNC TEST.lua", "Violence District.lua"]);
        search.TextChanged += (_, _) =>
        {
            var query = search.Text.Trim().ToLowerInvariant();
            list.Items.Clear();
            var all = new[] { "AimBot.lua", "Animation Menu.lua", "Sambung Kata.lua", "UNC TEST.lua", "Violence District.lua" };
            list.Items.AddRange(all.Where(item => item.ToLowerInvariant().Contains(query)).Cast<object>().ToArray());
        };
        list.SelectedValueChanged += (_, _) =>
        {
            if (list.SelectedItem is string scriptName)
            {
                _editor.Text = $"-- {scriptName}\nprint('Loaded from script list')";
            }
        };

        scriptPanel.Controls.Add(list);
        scriptPanel.Controls.Add(search);

        var actionBar = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(4, 12, 4, 4), BackColor = Color.FromArgb(4, 8, 20) };
        actionBar.Controls.Add(CreateButton("Execute", () => _notificationService.Show("Execution simulated."), Color.FromArgb(99, 102, 241)));
        actionBar.Controls.Add(CreateButton("Open", OpenFile, Color.FromArgb(22, 163, 74)));
        actionBar.Controls.Add(CreateButton("Clear", () => _editor.Clear(), Color.FromArgb(8, 145, 178)));
        actionBar.Controls.Add(CreateButton("Kill Roblox", KillRoblox, Color.FromArgb(15, 23, 42)));

        _injectButton = CreateButton("Inject", InjectAsync, Color.FromArgb(37, 99, 235));
        _progressBar = new ProgressBar { Width = 130, Style = ProgressBarStyle.Marquee, Visible = false, MarqueeAnimationSpeed = 20 };
        actionBar.Controls.Add(_injectButton);
        actionBar.Controls.Add(_progressBar);

        root.Controls.Add(_editor, 0, 0);
        root.Controls.Add(scriptPanel, 1, 0);
        root.Controls.Add(actionBar, 0, 1);
        root.SetColumnSpan(actionBar, 2);

        Controls.Add(root);
    }

    public void RefreshApiList()
    {
    }

    private void OpenFile()
    {
        using var dialog = new OpenFileDialog { Filter = "Lua scripts|*.lua;*.txt|All files|*.*" };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _editor.Text = File.ReadAllText(dialog.FileName);
            _notificationService.Show($"Loaded {Path.GetFileName(dialog.FileName)}");
        }
    }

    private void KillRoblox()
    {
        var targets = Process.GetProcessesByName("RobloxPlayerBeta");
        if (targets.Length == 0)
        {
            _notificationService.Show("Roblox process not found.");
            return;
        }

        foreach (var process in targets)
        {
            process.Kill(true);
        }

        _notificationService.Show("Roblox process terminated.");
    }

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

    private static Button CreateButton(string text, Action onClick, Color color)
    {
        var button = new Button { Text = text, Width = 110, Height = 34, FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White, Margin = new Padding(0, 0, 8, 0) };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => onClick();
        return button;
    }
}
