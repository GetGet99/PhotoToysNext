using PhotoToysV2.Controls;
using System.Numerics;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double W = 300;
    double H = 100;
    Color Color = `Colors.DarkRed`;
    <root Toolbar
        createResizer=<CreateResizer Value=`OutputRect` Value=>`OutputRect` />
    >
        <HStack Spacing=16>
            <TextBlock Text="Width" CenterV />
            <NumberBox Value=`Math.Round(W, 2)` Value=>`W` />
            <TextBlock Text="Height" CenterV />
            <NumberBox Value=`Math.Round(H, 2)` Value=>`H` />
            <TextBlock Text="Color" CenterV />
            <ColorButton Color<=>`Color` Width=32 Height=32 CenterV />
        </HStack>
    </root>
    """)]
partial class RectangleEffect : Card, ICreateImageEffectPreview, IImageEffectSelectNotify, IImageEffectApplyNotify
{
    private Rect OutputRect
    {
        get
        {
            return new(0, 0, W, H);
        }
        set
        {
            W = value.Width;
            H = value.Height;
        }
    }
    CreateResizer createResizer = null!;
    public string DisplayName => "Rectangle";

    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;
    public ICanvasImage GetEffect()
    {
        using var ds = NewDrawing(new(W, H), out var result);
        ds.Clear(Color);
        return result;
    }
    public ICanvasImage GetPreview()
    {
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
            SourceRectangle = new(0, 0, W, H)
        };

        return crop;
    }

    public RectangleEffect()
    {
        Init();
        Effect(() => ParametersUpdated?.Invoke(), WProp, HProp, ColorProp);
    }
    public ICanvasImage GetExample()
    {
        var color = new ColorSourceEffect
        {
            Color = Colors.DarkRed
        };

        var crop = new CropEffect
        {
            Source = color,
            SourceRectangle = new(0, 0, 150, 100)
        };

        return crop;
    }

    public void Selected()
    {
        ResizerService.Instance.Resizer = createResizer;
    }

    public void Deselected()
    {
        if (ResizerService.Instance.Resizer == createResizer)
            ResizerService.Instance.Resizer = null;
    }

    public void Applied()
    {
        createResizer.Reset();
    }
}
