namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double Intensity = 0;
    <root Toolbar>
        <HStack Spacing=16>
            <TextBlock Text="Intensity" CenterV />
            <Slider Value=100 Value=>`Intensity` Minimum=0 Maximum=100 Width=200 CenterV />
        </HStack>
    </root>
    """)]
partial class Grayscale : Card, ISingleImageEffect
{
    public string DisplayName => "Grayscale";

    public IEnumerable<string> Keywords => ["HSV"];

    public event Action? ParametersUpdated;
    public ICanvasImage GetEffect(ICanvasImage input)
    {
        var sat = new SaturationEffect
        {
            Source = input,
            Saturation = Math.Clamp((100 - (float)Intensity) / 100, 0, 1)
        };

        return sat;
    }

    public ICanvasImage GetExample(ICanvasImage input)
    {
        var sat = new SaturationEffect
        {
            Source = input,
            Saturation = 0
        };

        return sat;
    }

    public Grayscale()
    {
        Init();
        IntensityProp.Watch(_ => ParametersUpdated?.Invoke());
    }
}
