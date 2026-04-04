using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Services;

namespace ExterExecutor.App.Features.Dashboard;

internal sealed class DashboardView : UserControl
{
    private readonly Label _statusValue;

    public DashboardView(AppSettings settings, AppStateService appStateService)
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

        _statusValue = CreateCard(layout, "Runtime", appStateService.Status.ToString(), Color.FromArgb(56, 189, 248), 0, 0);
        CreateCard(layout, "User", settings.Username, Color.FromArgb(129, 140, 248), 1, 0);
        CreateCard(layout, "Theme", settings.Theme, Color.FromArgb(74, 222, 128), 0, 1);
        CreateCard(layout, "APIs", settings.ApiEndpoints.Count.ToString(), Color.FromArgb(251, 113, 133), 1, 1);

        Controls.Add(layout);

        appStateService.StatusChanged += status => _statusValue.Text = status.ToString();
    }

    private static Label CreateCard(TableLayoutPanel layout, string title, string value, Color accent, int col, int row)
    {
        var panel = new Panel { Margin = new Padding(12), Dock = DockStyle.Fill, BackColor = Color.FromArgb(31, 41, 55) };
        var titleLabel = new Label { Text = title, ForeColor = Color.FromArgb(148, 163, 184), Dock = DockStyle.Top, Height = 28, Padding = new Padding(16, 10, 0, 0) };
        var valueLabel = new Label { Text = value, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 15, FontStyle.Bold), Dock = DockStyle.Fill, Padding = new Padding(16, 0, 0, 0) };

        panel.Controls.Add(valueLabel);
        panel.Controls.Add(titleLabel);
        panel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = accent });
        layout.Controls.Add(panel, col, row);
        return valueLabel;
    }
}
