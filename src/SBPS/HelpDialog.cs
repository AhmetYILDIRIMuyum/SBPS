namespace SBPS;

internal sealed class HelpDialog : Form
{
    internal HelpDialog(
        string title,
        string intro,
        string closeText,
        string footerText,
        IReadOnlyList<(string Title, string Body)> sections)
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScroll = false;
        BackColor = PremiumPalette.Canvas;
        ClientSize = new Size(980, 560);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = title;

        var header = new GradientHeaderPanel
        {
            Dock = DockStyle.Top,
            Height = 72,
            Padding = new Padding(22, 12, 22, 10),
        };
        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            RowCount = 2,
        };
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
        headerLayout.Controls.Add(new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 16F),
            Text = title,
            TextAlign = ContentAlignment.BottomLeft,
        }, 0, 0);
        headerLayout.Controls.Add(new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(205, 224, 250),
            Font = new Font("Segoe UI", 9F),
            Text = intro,
            TextAlign = ContentAlignment.TopLeft,
        }, 0, 1);
        header.Controls.Add(headerLayout);

        var grid = new TableLayoutPanel
        {
            AutoScroll = false,
            BackColor = PremiumPalette.Canvas,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 12, 16, 4),
            RowCount = 3,
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        for (var index = 0; index < sections.Count && index < 6; index++)
        {
            var section = sections[index];
            grid.Controls.Add(
                new HelpSectionCard(index + 1, section.Title, section.Body),
                index % 2,
                index / 2);
        }

        var footer = new Panel
        {
            AutoScroll = false,
            BackColor = PremiumPalette.Canvas,
            Dock = DockStyle.Bottom,
            Height = 52,
            Padding = new Padding(18, 8, 18, 10),
        };
        var footerRule = new Panel
        {
            BackColor = PremiumPalette.Border,
            Dock = DockStyle.Top,
            Height = 1,
        };
        var footerTextLabel = new Label
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            AutoSize = false,
            Font = new Font("Segoe UI", 8F),
            ForeColor = PremiumPalette.Muted,
            Location = new Point(18, 12),
            Size = new Size(700, 28),
            Text = footerText,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var closeButton = new PremiumButton
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            DialogResult = DialogResult.Cancel,
            Location = new Point(840, 8),
            Size = new Size(112, 34),
            Text = closeText,
            Variant = PremiumButtonVariant.Primary,
        };
        footer.Resize += (_, _) =>
        {
            closeButton.Left = footer.ClientSize.Width - closeButton.Width - 18;
            footerTextLabel.Width = Math.Max(160, closeButton.Left - footerTextLabel.Left - 12);
        };
        footer.Controls.Add(footerTextLabel);
        footer.Controls.Add(closeButton);
        footer.Controls.Add(footerRule);

        Controls.Add(grid);
        Controls.Add(footer);
        Controls.Add(header);
        AcceptButton = closeButton;
        CancelButton = closeButton;
    }

    private sealed class HelpSectionCard : PremiumCardPanel
    {
        internal HelpSectionCard(int index, string title, string body)
        {
            AutoScroll = false;
            Dock = DockStyle.Fill;
            Margin = new Padding(6);
            Padding = new Padding(12, 8, 12, 8);

            var titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 9F),
                ForeColor = PremiumPalette.Navy,
                Height = 22,
                Text = $"{index:00}   {title}",
                TextAlign = ContentAlignment.MiddleLeft,
            };
            var bodyLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8.25F),
                ForeColor = PremiumPalette.Text,
                Text = body,
            };
            Controls.Add(bodyLabel);
            Controls.Add(titleLabel);
        }
    }
}
