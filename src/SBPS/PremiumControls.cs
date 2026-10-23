using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SBPS;

internal static class PremiumPalette
{
    public static readonly Color Canvas = Color.FromArgb(244, 247, 252);
    public static readonly Color Surface = Color.White;
    public static readonly Color Navy = Color.FromArgb(13, 27, 52);
    public static readonly Color NavySoft = Color.FromArgb(31, 49, 79);
    public static readonly Color Primary = Color.FromArgb(39, 98, 255);
    public static readonly Color PrimaryLight = Color.FromArgb(91, 140, 255);
    public static readonly Color Cyan = Color.FromArgb(23, 201, 184);
    public static readonly Color Text = Color.FromArgb(25, 39, 65);
    public static readonly Color Muted = Color.FromArgb(105, 120, 145);
    public static readonly Color Border = Color.FromArgb(222, 229, 240);
    public static readonly Color SoftBlue = Color.FromArgb(235, 241, 255);
    public static readonly Color Danger = Color.FromArgb(218, 62, 82);
}

internal enum PremiumButtonVariant
{
    Primary,
    Secondary,
    Danger,
}

internal sealed class PremiumButton : Button
{
    private bool _hovered;
    private bool _pressed;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal PremiumButtonVariant Variant { get; set; } = PremiumButtonVariant.Secondary;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int CornerRadius { get; set; } = 9;

    public PremiumButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        UseVisualStyleBackColor = false;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI Semibold", 9.5F);
        MinimumSize = new Size(96, 38);
        Padding = new Padding(12, 0, 12, 0);
        TabStop = true;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hovered = false;
        _pressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        _pressed = true;
        Invalidate();
        base.OnMouseDown(mevent);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        _pressed = false;
        Invalidate();
        base.OnMouseUp(mevent);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        Invalidate();
        base.OnEnabledChanged(e);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedRectangle(bounds, CornerRadius);

        var (start, end, text, border) = ResolveColors();
        using var background = new LinearGradientBrush(bounds, start, end, LinearGradientMode.Horizontal);
        pevent.Graphics.FillPath(background, path);

        if (border != Color.Transparent)
        {
            using var borderPen = new Pen(border, 1F);
            pevent.Graphics.DrawPath(borderPen, path);
        }

        TextRenderer.DrawText(
            pevent.Graphics,
            Text,
            Font,
            bounds,
            text,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis |
            TextFormatFlags.SingleLine);

        if (Focused && ShowFocusCues)
        {
            var focusBounds = Rectangle.Inflate(bounds, -4, -4);
            ControlPaint.DrawFocusRectangle(pevent.Graphics, focusBounds, text, Color.Transparent);
        }
    }

    private (Color Start, Color End, Color Text, Color Border) ResolveColors()
    {
        if (!Enabled)
        {
            return (
                Color.FromArgb(224, 229, 237),
                Color.FromArgb(224, 229, 237),
                Color.FromArgb(148, 158, 175),
                Color.Transparent);
        }

        var intensity = _pressed ? -22 : _hovered ? 10 : 0;
        return Variant switch
        {
            PremiumButtonVariant.Primary => (
                Shift(PremiumPalette.Primary, intensity),
                Shift(PremiumPalette.PrimaryLight, intensity),
                Color.White,
                Color.Transparent),
            PremiumButtonVariant.Danger => (
                Shift(Color.White, _pressed ? -8 : _hovered ? -3 : 0),
                Shift(Color.White, _pressed ? -8 : _hovered ? -3 : 0),
                PremiumPalette.Danger,
                Color.FromArgb(238, 183, 191)),
            _ => (
                Shift(Color.White, _pressed ? -8 : _hovered ? -3 : 0),
                Shift(Color.White, _pressed ? -8 : _hovered ? -3 : 0),
                PremiumPalette.Text,
                PremiumPalette.Border),
        };
    }

    private static Color Shift(Color color, int amount) => Color.FromArgb(
        color.A,
        Math.Clamp(color.R + amount, 0, 255),
        Math.Clamp(color.G + amount, 0, 255),
        Math.Clamp(color.B + amount, 0, 255));

    internal static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var diameter = Math.Max(1, radius * 2);
        var path = new GraphicsPath();
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}

internal sealed class PremiumCardPanel : Panel
{
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int CornerRadius { get; set; } = 14;

    public PremiumCardPanel()
    {
        DoubleBuffered = true;
        BackColor = PremiumPalette.Surface;
        Padding = new Padding(18);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = PremiumButton.RoundedRectangle(bounds, CornerRadius);
        using var fill = new SolidBrush(BackColor);
        using var border = new Pen(PremiumPalette.Border, 1F);
        e.Graphics.FillPath(fill, path);
        e.Graphics.DrawPath(border, path);
        base.OnPaint(e);
    }
}

internal sealed class GradientHeaderPanel : Panel
{
    public GradientHeaderPanel()
    {
        DoubleBuffered = true;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        using var background = new LinearGradientBrush(
            ClientRectangle,
            PremiumPalette.Navy,
            Color.FromArgb(28, 69, 130),
            LinearGradientMode.Horizontal);
        e.Graphics.FillRectangle(background, ClientRectangle);

        using var glow = new SolidBrush(Color.FromArgb(28, PremiumPalette.Cyan));
        e.Graphics.FillEllipse(glow, Width - 250, -120, 360, 300);
    }
}

internal sealed class PremiumProgressBar : Control
{
    private int _minimum;
    private int _maximum = 100;
    private int _value;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int Minimum
    {
        get => _minimum;
        set
        {
            _minimum = value;
            if (_maximum < _minimum)
            {
                _maximum = _minimum;
            }

            Value = _value;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int Maximum
    {
        get => _maximum;
        set
        {
            _maximum = Math.Max(value, _minimum);
            Value = _value;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, _minimum, _maximum);
            Invalidate();
        }
    }

    public PremiumProgressBar()
    {
        DoubleBuffered = true;
        Height = 8;
        MinimumSize = new Size(100, 8);
        MaximumSize = new Size(10000, 8);
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var trackBounds = new Rectangle(0, 0, Width - 1, Height - 1);
        using var trackPath = PremiumButton.RoundedRectangle(trackBounds, Height / 2);
        using var trackBrush = new SolidBrush(Color.FromArgb(226, 232, 242));
        e.Graphics.FillPath(trackBrush, trackPath);

        var range = Math.Max(1, _maximum - _minimum);
        var ratio = (double)(_value - _minimum) / range;
        var fillWidth = (int)Math.Round(Width * ratio);
        if (fillWidth <= 0)
        {
            return;
        }

        var fillBounds = new Rectangle(0, 0, Math.Max(Height, fillWidth), Height - 1);
        using var fillPath = PremiumButton.RoundedRectangle(fillBounds, Height / 2);
        using var fillBrush = new LinearGradientBrush(
            fillBounds,
            PremiumPalette.Primary,
            PremiumPalette.Cyan,
            LinearGradientMode.Horizontal);
        e.Graphics.FillPath(fillBrush, fillPath);
    }
}
