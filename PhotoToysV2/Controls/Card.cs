namespace PhotoToysV2;

[QuickMarkup("""
    Brush? Background = null;
    <setup>
    Padding = new(8);
    CornerRadius = new(16);
    var bgBrushDynamic = ThemeResources.Get<Brush>("CardBackgroundFillColorDefaultBrush", this).CreateReadOnlyReference();
    var borderBrushDynamic = ThemeResources.Get<Brush>("CardStrokeColorDefaultBrush", this).CreateReadOnlyReference();
    var cornerRadius = this.CreateReadOnlyRefrence<CornerRadius>(CornerRadiusProperty);
    var padding = this.CreateReadOnlyRefrence<Thickness>(PaddingProperty);
    </setup>
    <root
        Content=<Border
            Padding=`padding.Value`
            BorderThickness=1
            CornerRadius=`cornerRadius.Value`
            Child=`Child`
            Background=`Background ?? bgBrushDynamic.Value`
            BorderBrush=`borderBrushDynamic.Value`
        />
    />
    """)]
[ContentProperty(Name = "Child")]
partial class Card : UserControl
{
    // need to keep them explicit as generator cannot find the field.
    Reference<UIElement?> ChildProp => field ??= Ref<UIElement?>(null);
    public UIElement? Child
    {
        get => ChildProp.Value;
        set => ChildProp.Value = value;
    }
    public void Toolbar()
    {
        Padding = new(24, 8, 24, 8);
        this.FullRounded();
    }
}
