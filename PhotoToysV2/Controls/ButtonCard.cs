using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI.Xaml.Automation;

namespace PhotoToysV2;

[QuickMarkup("""
    using Microsoft.Graphics.Canvas.UI.Xaml;
    <setup>
    var style = (Style)App.Current.Resources["ButtonCheckBoxButtonStyle"];
    var root = Window.Current.Content as Frame;
    </setup>
    <root
        Style=/-style-/
        Content = canvas = <CanvasControl Width=150 Height=100 Draw+=/-DrawHandler-/ />
    />
    """)]
partial class ButtonCard : CheckBox
{
    public ButtonCard()
    {
        Init();
    }
    public string? Text
    {
        get; set
        {
            field = value;
            AutomationProperties.SetName(this, value);
            canvas.Invalidate();
        }
    }
    static readonly CanvasDevice device = CanvasDevice.GetSharedDevice();
    public ICanvasImage? ExampleImage
    {
        get; set
        {
            field = value;
            canvas.Invalidate();
        }
    }
    static readonly CanvasTextFormat Format = new()
    {
        FontFamily = new("Segoe UI Variable"),
        FontSize = 14,
        HorizontalAlignment = CanvasHorizontalAlignment.Right,
        VerticalAlignment = CanvasVerticalAlignment.Bottom,
    };
    static readonly CanvasLinearGradientBrush SharedGraident = new(device,
        [
            new CanvasGradientStop
            {
                Position = 0f,
                Color = Colors.Transparent
            },
            new CanvasGradientStop
            {
                Position = 1f,
                Color = Color.FromArgb(180, 0, 0, 0) // semi-transparent black
            }
        ])
    {
        StartPoint = new Vector2(0, 0),
        EndPoint = new Vector2(0, 100)
    };
    void DrawHandler(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (ExampleImage is { } img)
        {
            var bounds = img.Bounds;
            var scale = Math.Max(
                canvas.ActualWidth / bounds.Width,
                canvas.ActualHeight / bounds.Height
            );
            var newSize = new Size(bounds.Width * scale, bounds.Height * scale);
            var newLoc = new Point((canvas.ActualWidth - newSize.Width) / 2, (canvas.ActualHeight - newSize.Height) / 2);
            args.DrawingSession.DrawImage(img, new Rect(newLoc, newSize), bounds);
            args.DrawingSession.FillRectangle(
                new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight),
                SharedGraident
            );
        }
        args.DrawingSession.DrawText(Text, new Rect(16, 16, Math.Max(0, canvas.ActualWidth - 32), Math.Max(0, canvas.ActualHeight - 32)), Colors.White, Format);
    }
}