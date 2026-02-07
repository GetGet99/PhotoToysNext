namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double Hue = 0;
    double Saturation = 0;
    double Brightness = 0;
    <root Toolbar>
        <HStack Spacing=16>
            <TextBlock Text="Hue" CenterV />
            <SliderFromCenter Value=>/-Hue-/ Minimum=-180 Maximum=180 Width=200 CenterV />
            <TextBlock Text="Saturation" CenterV />
            <SliderFromCenter Value=1 Value=>/-Saturation-/ Minimum=0 Maximum=2 StepFrequency=0.01 Width=200 CenterV />
            <TextBlock Text="Brightness" CenterV />
            <SliderFromCenter Value=>/-Brightness-/ Minimum=-3 Maximum=3 StepFrequency=0.01 Width=200 CenterV />
        </HStack>
    </root>
    """)]
partial class AdjustColor : Card, ISingleImageEffect
{
    public string DisplayName => "Adjust Color";

    public IEnumerable<string> Keywords => ["HSV"];

    public event Action? ParametersUpdated;
    public ICanvasImage GetEffect(ICanvasImage input)
    {
        var hue = new HueRotationEffect
        {
            Source = input,
            Angle = (float)Hue * MathF.PI / 180f
        };

        var sat = new SaturationEffect
        {
            Source = hue,
            Saturation = (float)Saturation
        };
        float brightness = (float)Math.Pow(2, Brightness);

        var bright = new ColorMatrixEffect
        {
            Source = sat,
            ColorMatrix = new Matrix5x4
            {
                M11 = brightness, // R
                M22 = brightness, // G
                M33 = brightness, // B
                M44 = 1.0f    // A
            }
        };

        return bright;
    }

    public AdjustColor()
    {
        Init();
        Effect(() => ParametersUpdated?.Invoke(),
            HueProp, SaturationProp, BrightnessProp
        );
    }
    public ICanvasImage GetExample(ICanvasImage input)
    {
        var hue = new HueRotationEffect
        {
            Source = input,
            Angle = 30 * MathF.PI / 180f
        };

        var sat = new SaturationEffect
        {
            Source = hue,
            Saturation = 1.5f
        };
        float brightness = MathF.Pow(2, 0.5f);

        var bright = new ColorMatrixEffect
        {
            Source = sat,
            ColorMatrix = new Matrix5x4
            {
                M11 = brightness, // R
                M22 = brightness, // G
                M33 = brightness, // B
                M44 = 1.0f    // A
            }
        };

        return bright;
    }
}
