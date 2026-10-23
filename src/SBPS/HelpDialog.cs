namespace SBPS;

internal sealed class HelpDialog : Form
{
    internal HelpDialog(string title, string content, string closeText)
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = PremiumPalette.Canvas;
        ClientSize = new Size(760, 640);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(620, 520);
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = title;

        var header = new GradientHeaderPanel
        {
            Dock = DockStyle.Top,
            Height = 118,
            Padding = new Padding(28, 20, 28, 18),
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
            AutoSize = true,
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 20F),
            Text = title,
            TextAlign = ContentAlignment.BottomLeft,
        }, 0, 0);
        headerLayout.Controls.Add(new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(205, 224, 250),
            Font = new Font("Segoe UI", 9.5F),
            Text = "SBPS  •  Smart Batch Printing Studio",
            TextAlign = ContentAlignment.TopLeft,
        }, 0, 1);
        header.Controls.Add(headerLayout);

        var card = new PremiumCardPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(22),
        };
        card.Controls.Add(new RichTextBox
        {
            BackColor = PremiumPalette.Surface,
            BorderStyle = BorderStyle.None,
            Cursor = Cursors.Arrow,
            DetectUrls = true,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            ForeColor = PremiumPalette.Text,
            ReadOnly = true,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            TabStop = false,
            Text = content,
        });

        var body = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = PremiumPalette.Canvas,
            Padding = new Padding(22, 20, 22, 8),
        };
        body.Controls.Add(card);

        var footer = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 72,
            Padding = new Padding(22, 12, 22, 16),
        };
        var closeButton = new PremiumButton
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            DialogResult = DialogResult.Cancel,
            Location = new Point(footer.ClientSize.Width - 142, 12),
            Size = new Size(120, 42),
            Text = closeText,
            Variant = PremiumButtonVariant.Primary,
        };
        footer.Resize += (_, _) => closeButton.Left = footer.ClientSize.Width - closeButton.Width - 22;
        footer.Controls.Add(closeButton);

        Controls.Add(body);
        Controls.Add(footer);
        Controls.Add(header);
        AcceptButton = closeButton;
        CancelButton = closeButton;
    }
}
