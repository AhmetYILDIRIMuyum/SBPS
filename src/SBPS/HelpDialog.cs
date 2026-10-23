namespace SBPS;

internal sealed class HelpDialog : Form
{
    private readonly Panel _scroll;
    private readonly FlowLayoutPanel _sections;

    internal HelpDialog(
        string title,
        string intro,
        string closeText,
        string footerText,
        IReadOnlyList<(string Title, string Body)> sections)
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = PremiumPalette.Canvas;
        ClientSize = new Size(820, 680);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(680, 540);
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

        _scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = PremiumPalette.Canvas,
            Padding = new Padding(22, 16, 8, 8),
        };
        var introLabel = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 10F),
            ForeColor = PremiumPalette.Muted,
            Margin = new Padding(2, 0, 0, 14),
            Text = intro,
        };
        _sections = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = PremiumPalette.Canvas,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };
        _sections.Controls.Add(introLabel);
        for (var index = 0; index < sections.Count; index++)
        {
            var section = sections[index];
            _sections.Controls.Add(new HelpSectionCard(index + 1, section.Title, section.Body));
        }

        _scroll.Controls.Add(_sections);
        _scroll.Resize += (_, _) => LayoutSections();

        var footer = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 76,
            Padding = new Padding(22, 10, 22, 16),
            BackColor = PremiumPalette.Canvas,
        };
        var footerRule = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = PremiumPalette.Border,
        };
        var footerTextLabel = new Label
        {
            AutoSize = false,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            Font = new Font("Segoe UI", 8F),
            ForeColor = PremiumPalette.Muted,
            Location = new Point(22, 18),
            Size = new Size(520, 36),
            Text = footerText,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var closeButton = new PremiumButton
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            DialogResult = DialogResult.Cancel,
            Location = new Point(footer.ClientSize.Width - 148, 16),
            Size = new Size(126, 42),
            Text = closeText,
            Variant = PremiumButtonVariant.Primary,
        };
        footer.Resize += (_, _) =>
        {
            closeButton.Left = footer.ClientSize.Width - closeButton.Width - 22;
            footerTextLabel.Width = Math.Max(160, closeButton.Left - footerTextLabel.Left - 16);
        };
        footer.Controls.Add(footerTextLabel);
        footer.Controls.Add(closeButton);
        footer.Controls.Add(footerRule);

        Controls.Add(_scroll);
        Controls.Add(footer);
        Controls.Add(header);
        AcceptButton = closeButton;
        CancelButton = closeButton;
        Load += (_, _) => LayoutSections();
    }

    private void LayoutSections()
    {
        var width = Math.Max(
            280,
            _scroll.ClientSize.Width - _scroll.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
        _sections.Width = width;
        foreach (Control control in _sections.Controls)
        {
            switch (control)
            {
                case HelpSectionCard card:
                    card.SetWidth(width);
                    break;
                case Label label:
                    label.MaximumSize = new Size(width - 4, 0);
                    break;
            }
        }
    }

    private sealed class HelpSectionCard : PremiumCardPanel
    {
        private readonly Label _title;
        private readonly Label _body;

        internal HelpSectionCard(int index, string title, string body)
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(0, 0, 0, 12);
            Padding = new Padding(16, 14, 18, 14);

            var layout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                ColumnCount = 2,
                Margin = Padding.Empty,
                RowCount = 2,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var badge = new Label
            {
                BackColor = PremiumPalette.SoftBlue,
                Font = new Font("Segoe UI Semibold", 8.5F),
                ForeColor = PremiumPalette.Primary,
                Margin = new Padding(0, 1, 8, 0),
                Size = new Size(34, 24),
                Text = index.ToString("00"),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            _title = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = PremiumPalette.Navy,
                Margin = new Padding(0, 2, 0, 6),
                Text = title,
            };
            _body = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = PremiumPalette.Text,
                Margin = Padding.Empty,
                Text = body,
            };
            layout.Controls.Add(badge, 0, 0);
            layout.SetRowSpan(badge, 2);
            layout.Controls.Add(_title, 1, 0);
            layout.Controls.Add(_body, 1, 1);
            Controls.Add(layout);
        }

        internal void SetWidth(int width)
        {
            Width = width;
            var textWidth = Math.Max(180, width - Padding.Horizontal - 50);
            if (Controls[0] is TableLayoutPanel layout)
            {
                layout.Width = Math.Max(180, width - Padding.Horizontal);
            }
            _title.MaximumSize = new Size(textWidth, 0);
            _body.MaximumSize = new Size(textWidth, 0);
        }
    }
}
