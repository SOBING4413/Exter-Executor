using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Editor;

internal sealed class ScriptEditorView : UserControl
{
    private readonly RichTextBox _editor;

    public ScriptEditorView(AppSettings settings, NotificationService notificationService)
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(17, 24, 39);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(12, 10, 12, 10),
            BackColor = Color.FromArgb(31, 41, 55)
        };

        var executeButton = CreateButton("Execute", () => notificationService.Show("Script queued for execution."));
        var clearButton = CreateButton("Clear", () => _editor.Clear());

        toolbar.Controls.Add(executeButton);
        toolbar.Controls.Add(clearButton);

        _editor = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.FromArgb(226, 232, 240),
            Font = new Font("Cascadia Code", settings.Editor.FontSize),
            WordWrap = settings.Editor.WordWrap,
            Text = "-- Welcome to the modernized editor\nprint('Hello from Exter Executor')"
        };

        Controls.Add(_editor);
        Controls.Add(toolbar);
    }

    private static Button CreateButton(string text, Action onClick)
    {
        var button = new Button
        {
            Text = text,
            Width = 120,
            Height = 30,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(59, 130, 246),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Margin = new Padding(0, 0, 10, 0)
        };

        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => onClick();
        return button;
    }
}
