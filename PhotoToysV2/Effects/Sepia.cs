namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double Intensity = 50;
    <root Toolbar>
        <HStack Spacing=16>
            <TextBlock Text="Intensity" CenterV />
            <Slider Value<=>`Intensity`
                    Minimum=0 Maximum=100 Width=200 CenterV />
        </HStack>
    </root>
    """)]
partial class Sepia : Card, ISingleImageEffect
{
    public string DisplayName => "Sepia";
    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;

    public ICanvasImage GetEffect(ICanvasImage input)
    {
        float t = (float)Intensity / 100f;

        return new SepiaEffect
        {
            Source = input,
            Intensity = t
        };
    }

    public ICanvasImage GetExample(ICanvasImage input)
    {
        return new SepiaEffect
        {
            Source = input,
            Intensity = 0.5f
        };
    }

    public Sepia()
    {
        Init();
        IntensityProp.Watch(_ => ParametersUpdated?.Invoke());
    }
}