using Get.Data.XACL;
using System.Numerics;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    private Thickness Thickness;
    private Color Color = /-Colors.Black-/;
    <root Toolbar
        // marginResizer = <MarginResizer Value=/-Thickness-/ Value=>/-Thickness-/ />
    >
        <HStack Spacing=16>
             <TextBlock Text="Left:" CenterV />
             <NumberBox Value=/-Math.Round(Left, 2)-/ Value=>/-Left-/ />
             <TextBlock Text="Top:" CenterV />
             <NumberBox Value=/-Math.Round(Top, 2)-/ Value=>/-Top-/ />
             <TextBlock Text="Right:" CenterV />
             <NumberBox Value=/-Math.Round(Right, 2)-/ Value=>/-Right-/ />
             <TextBlock Text="Bottom:" CenterV />
             <NumberBox Value=/-Math.Round(Bottom, 2)-/ Value=>/-Bottom-/ />
            <TextBlock Text="Color:" CenterV />
            <ColorButton Color=/-Color-/ Color=>/-Color-/ Width=32 Height=32 />
        </HStack>
    </root>
    """)]
partial class BorderEf : Card, ISingleImageEffectPreview, IImageEffectSelectNotify, IImageEffectApplyNotify
{
    double Left { get => Thickness.Left; set => Thickness = Thickness with { Left = value }; }
    double Top { get => Thickness.Top; set => Thickness = Thickness with { Top = value }; }
    double Right { get => Thickness.Right; set => Thickness = Thickness with { Right = value }; }
    double Bottom { get => Thickness.Bottom; set => Thickness = Thickness with { Bottom = value }; }
    public string DisplayName => "Border";
    private MarginResizer marginResizer = null!;

    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;
    // while editing, do not change input image
    public ICanvasImage GetExample(ICanvasImage input)
        => GetBorderEffect(input,
            new(Math.Max((input.Bounds.Width + input.Bounds.Height) / 2 * 0.25f, 20)),
            Colors.Black
        );
    public ICanvasImage GetPreview(ICanvasImage input)
    {
        return GetBorderEffect(input, Thickness, Color);
        var bounds = input.Bounds;
        //return new Transform2DEffect
        //{
        //    Source = new CropEffect
        //    {
        //        Source = input,
        //        SourceRectangle = new Rect(bounds.Left - Left, bounds.Top - Top, bounds.Width + Left + Right, bounds.Height + Top + Bottom)
        //    },
        //    TransformMatrix = Matrix3x2.CreateTranslation((float)Left, (float)Top)
        //};
        //return new CropEffect
        //{
        //    Source = new BorderEffect
        //    {
        //        Source = input
        //    },
        //    SourceRectangle = new Rect(bounds.Left - Left, bounds.Top - Top, bounds.Width + Left + Right, bounds.Height + Top + Bottom)
        //};
        var target = new Rect(bounds.Left - Left, bounds.Top - Top, bounds.Width + Left + Right, bounds.Height + Top + Bottom);
        var alpha = Color.A;
        IGraphicsEffectSource color = new ColorSourceEffect
        {
            Color = Color with { A = 255 }
        };

        if (alpha is not 255)
        {
            color = new ColorMatrixEffect
            {
                Source = color,
                ColorMatrix = new Matrix5x4(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, alpha / 255f,
                    0, 0, 0, 0
                )
            };
        }

        var crop = new CropEffect
        {
            Source = color,
            SourceRectangle = new(0, 0, target.Width, target.Height)
        };
        return crop;
    }
    public ICanvasImage GetEffect(ICanvasImage input)
        //=> GetBorderEffect(input,
        //    new(Math.Max((input.Bounds.Width + input.Bounds.Height) / 2 * 0.25f, 20)),
        //    Colors.Black
        //);
        => ReferenceTracker.NoCapture(() => GetBorderEffect(input, Thickness, Color));
    ICanvasImage GetBorderEffect(ICanvasImage input, Thickness thickness, Color color)
    {
        var bounds = input.Bounds;
        var totalWidth = bounds.Width + thickness.Left + thickness.Right;
        using var ds = NewDrawing(
            new Size(
                totalWidth,
                bounds.Height + thickness.Top + thickness.Bottom
            ),
            out var renderTarget
        );
        ds.FillRectangle(0, 0,
            (float)totalWidth, (float)thickness.Top, color);
        ds.FillRectangle(0, (float)(thickness.Top + bounds.Height),
            (float)totalWidth, (float)thickness.Bottom, color);
        ds.FillRectangle(
            0, (float)thickness.Top,
            (float)thickness.Left, (float)bounds.Height, color);
        ds.FillRectangle(
            (float)(thickness.Left + bounds.Width), (float)thickness.Top,
            (float)thickness.Right, (float)bounds.Height, color);
        ds.DrawImage(input, (float)thickness.Left, (float)thickness.Top);
        return renderTarget;
        //return new CompositeEffect()
        //{
        //    Sources =
        //    {
        //        new Transform2DEffect {
        //            Source = input,
        //            TransformMatrix = Matrix3x2.CreateTranslation((float)thickness.Left, (float)thickness.Top)
        //        },
        //        renderTarget
        //    },
        //};
    }

    public BorderEf()
    {
        Init();
        ThicknessProp.Watch(_ => ParametersUpdated?.Invoke());
    }

    public void Selected()
    {
        ResizerService.Instance.Resizer = marginResizer;
    }

    public void Deselected()
    {
        if (ResizerService.Instance.Resizer == marginResizer)
            ResizerService.Instance.Resizer = null;
    }

    public void Applied()
    {
        //marginResizer.Reset();
    }
}
