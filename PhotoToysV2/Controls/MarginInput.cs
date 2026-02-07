using Windows.UI.Xaml.Shapes;

namespace PhotoToysV2.Controls;

[QuickMarkup("""
    using Windows.UI.Xaml.Shapes;
    Thickness Value = /-default-/;
    <setup>
    var borderBrushDynamic = ThemeResources.Get<Brush>("TextFillColorPrimaryBrush", this).CreateReadOnlyReference();
    </setup>
    <root>
        <Rectangle Width=150 Height=100 Margin=/-new(50,16,50,16)-/ StrokeThickness=1 Stroke=/-borderBrushDynamic.Value-/ />
        <NumberBox Minimum=/-0-/ Value=/-Left-/ Value=>/-Left-/ Left CenterV Width=100 />
        <NumberBox Minimum=/-0-/ Value=/-Top-/ Value=>/-Top-/ Top CenterH Width=100 />
        <NumberBox Minimum=/-0-/ Value=/-Right-/ Value=>/-Right-/ Right CenterV Width=100 />
        <NumberBox Minimum=/-0-/ Value=/-Bottom-/ Value=>/-Bottom-/ Bottom CenterH Width=100 />
    </root>
    """)]
partial class MarginInput : Grid
{
    double Left { get => Value.Left; set => Value = Value with { Left = value }; }
    double Top { get => Value.Top; set => Value = Value with { Top = value }; }
    double Right { get => Value.Right; set => Value = Value with { Right = value }; }
    double Bottom { get => Value.Bottom; set => Value = Value with { Bottom = value }; }
    public MarginInput()
    {
        Rectangle r = new();
        Init();
    }
}
