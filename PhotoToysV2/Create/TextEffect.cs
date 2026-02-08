using Microsoft.Graphics.Canvas.Text;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    using Microsoft.Graphics.Canvas.Text;
    string Text = "Sample Text";
    string FontFamily = "";
    Color Color = `Colors.Red`;
    bool AutoSize = true;
    float FontSize = 28;
    double W = 200;
    double H = 100;
    <root Padding=`new(24,16,24,16)`
        createResizer=<CreateResizer Value<=>`OutputRect` IsVisible=`!AutoSize` />
    >
        <VStack Spacing=16>
            <HStack Spacing=16 CenterH>
                <ComboBox Width=100
                    ItemsSource=`CanvasTextFormat.GetSystemFontFamilies().ToList()`
                    SelectedIndex=0 SelectedValue=>`FontFamilyHelper` CenterV
                />
                fsCbb = <ComboBox IsEditable Width=75 SelectedIndex=10 TextSubmitted+=`(_, _) => fsCbbCallback()`
                    SelectionChanged+=`(_, _) => fsCbbCallback()` CenterV
                >
                     8 9 10 11 12 14 16 18 20 24 28 36 48 72
                </ComboBox>
                <ColorButton Color<=>`Color` Width=32 Height=32 CenterV />
                <ToggleSwitch OnContent="Auto Size" OffContent="Auto Size" IsOn<=>`AutoSize` CenterV MinWidth=0 />
            </HStack>
            <TextBox Text<=>`Text` AcceptsReturn IsSpellCheckEnabled />
        </VStack>
    </root>
    """)]
partial class TextEffect : Card, ICreateImageEffect, IImageEffectSelectNotify, IImageEffectApplyNotify
{
    void fsCbbCallback()
    {
        if (float.TryParse(fsCbb.Text, out var fs) && fs > 0)
        {
            FontSize = fs;
        }
        else
        {
            fsCbb.Text = FontSize.ToString();
        }
    }
    private object FontFamilyHelper { set => FontFamily = (string)value; }
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
    public string DisplayName => "Text";

    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;
    public ICanvasImage GetEffect()
        => DrawText(AutoSize, OutputRect, Text, Color, FontFamily, FontSize);
    static ICanvasImage DrawText(bool AutoSize, Rect OutputRect, string Text, Color Color, string FontFamily, float FontSize)
    {
        Rect bounds;
        CanvasTextFormat format = new() {
            FontSize = FontSize,
            FontFamily = FontFamily,
            WordWrapping = AutoSize ? CanvasWordWrapping.NoWrap : CanvasWordWrapping.Wrap
        };
        if (AutoSize)
        {
            using var ds2 = NewDrawing(new(1, 1), out var result2);
            bounds = Create(ds2, 1, 1).DrawBounds;
            result2.Dispose();
        }
        else
        {
            bounds = OutputRect;
        }
        CanvasTextLayout Create(CanvasDrawingSession ds, float w, float h)
        {
            return new CanvasTextLayout(ds, Text, format, w, h);
        }
        using var ds = NewDrawing(new(bounds.Width, bounds.Height), out var result);

        ds.Clear(Colors.Transparent);
        ds.DrawTextLayout(Create(ds, (float)bounds.Width, (float)bounds.Height), -(float)bounds.Left, -(float)bounds.Top, Color);
        return result;
    }

    public TextEffect()
    {
        Init();
        Effect(() => ParametersUpdated?.Invoke(), WProp, HProp, TextProp, ColorProp, AutoSizeProp, FontFamilyProp, FontSizeProp);
    }
    public ICanvasImage GetExample()
        => DrawText(
            AutoSize: false,
            OutputRect: new(-156 / 2, -29 * 1.5, 156 * 2, 29 * 5),
            Text: "Sample Text",
            Color: Colors.Red,
            FontFamily: "",
            FontSize: 30f
        );

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
