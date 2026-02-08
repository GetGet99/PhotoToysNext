namespace PhotoToysV2.Controls;

[QuickMarkup("""
    Color Color;
    <root
        Background=<SolidColorBrush Color=`Color` />
        Flyout=<Flyout>
            <ColorPicker Color<=>`Color` IsAlphaEnabled />
        </Flyout>
    />
    """)]
partial class ColorButton : Button;
