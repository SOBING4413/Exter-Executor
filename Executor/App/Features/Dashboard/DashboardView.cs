namespace ExterExecutor.App.Features.Dashboard;

internal sealed class DashboardView : UserControl
{
    public DashboardView()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(17, 24, 39);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(24),
            BackColor = BackColor
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        layout.Controls.Add(CreateCard("Runtime", "Idle", Color.FromArgb(56, 189, 248)), 0, 0);
        layout.Controls.Add(CreateCard("Scripts", "12 installed", Color.FromArgb(129, 140, 248)), 1, 0);
        layout.Controls.Add(CreateCard("Workspace", "Healthy", Color.FromArgb(74, 222, 128)), 0, 1);
        layout.Controls.Add(CreateCard("Errors", "0 active", Color.FromArgb(251, 113, 133)), 1, 1);

        Controls.Add(layout);
    }

    private static Panel CreateCard(string title, string value, Color accent)
    {
        var panel = new Panel
        {
            Margin = new Padding(12),
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(31, 41, 55)
        };

        var titleLabel = new Label
        {
            Text = title,
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Dock = DockStyle.Top,
            Height = 28,
            Padding = new Padding(16, 10, 0, 0)
        };

        var valueLabel = new Label
        {
            Text = value,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 17, FontStyle.Bold),
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 0, 0, 0)
        };

        var accentBar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 4,
            BackColor = accent
        };

        panel.Controls.Add(valueLabel);
        panel.Controls.Add(titleLabel);
        panel.Controls.Add(accentBar);

        return panel;
    }
}
