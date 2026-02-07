using PhotoToysV2.Controls;
using System.Numerics;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double W = 300;
    double H = 200;
    Color Color = /-Colors.DarkRed-/;
    <root Toolbar
        createResizer=<CreateResizer Value=/-OutputRect-/ Value=>/-OutputRect-/ />
    >
        <HStack Spacing=16>
            <TextBlock Text="Width" CenterV />
            <NumberBox Value=/-Math.Round(W, 2)-/ Value=>/-W-/ />
            <TextBlock Text="Height" CenterV />
            <NumberBox Value=/-Math.Round(H, 2)-/ Value=>/-H-/ />
            <TextBlock Text="Color" CenterV />
            <Button Background=<SolidColorBrush Color=/-Color-/ /> Width=32 Height=32 CenterV>
                <.Flyout>
                    <Flyout>
                        <ColorPicker Color=/-Colors.DarkRed-/ Color=>/-Color-/ IsAlphaEnabled />
                    </Flyout>
                </.Flyout>
            </Button>
        </HStack>
    </root>
    """)]
partial class EllipseEffect : Card, ICreateImageEffect, IImageEffectSelectNotify, IImageEffectApplyNotify
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
    public string DisplayName => "Ellipse";

    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;
    public ICanvasImage GetEffect()
    {
        using var ds = NewDrawing(new(W, H), out var result);
        ds.Clear(Colors.Transparent);
        var rX = (float)(W / 2);
        var rY = (float)(H / 2);
        ds.FillEllipse(rX, rY, rX, rY, Color);
        return result;
    }

    public EllipseEffect()
    {
        Init();
        Effect(() => ParametersUpdated?.Invoke(), WProp, HProp, ColorProp);
    }
    public ICanvasImage GetExample()
    {
        var W = 300;
        var H = 200;
        using var ds = NewDrawing(new(W + 100, H + 100), out var result);
        ds.Clear(Colors.Transparent);
        var rX = (float)(W / 2);
        var rY = (float)(H / 2);
        ds.FillEllipse(rX + 50, rY + 50, rX, rY, Color);
        return result;
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
