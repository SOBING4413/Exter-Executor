using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Scripts;

internal sealed class ScriptLibraryView : UserControl
{
    public ScriptLibraryView(NotificationService notificationService)
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(17, 24, 39);
        DoubleBuffered = true;

        var container = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BackColor = BackColor,
            SplitterDistance = 320,
            IsSplitterFixed = false,
            Panel1MinSize = 220,
            Panel2MinSize = 260
        };

        var list = new ListBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(31, 41, 55),
            ForeColor = Color.FromArgb(226, 232, 240),
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 10)
        };
        list.Items.AddRange(["Infinite Yield", "Dex Explorer", "Simple Aimbot", "Animation Menu"]);

        var preview = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = new Font("Segoe UI", 10),
            Text = "Select a script from the left to view details and deploy to editor."
        };

        list.SelectedValueChanged += (_, _) =>
        {
            var selected = list.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            preview.Text = $"{selected}\r\n\r\nCategory: Utility\r\nStatus: Verified\r\n\r\nClick Deploy to open this script in the editor.";
        };

        var deployButton = new Button
        {
            Text = "Deploy",
            Dock = DockStyle.Bottom,
            Height = 36,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(16, 185, 129)
        };
        deployButton.FlatAppearance.BorderSize = 0;
        deployButton.Click += (_, _) => notificationService.Show("Script deployed to editor.");

        container.Panel1.Padding = new Padding(18);
        container.Panel2.Padding = new Padding(18);
        container.Panel1.Controls.Add(list);
        container.Panel2.Controls.Add(preview);
        container.Panel2.Controls.Add(deployButton);

        Controls.Add(container);
    }
}
