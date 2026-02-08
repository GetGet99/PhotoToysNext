namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double Intensity = 100;
    <root Toolbar>
        <HStack Spacing=16>
            <TextBlock Text="Intensity" CenterV />
            <Slider Value<=>`Intensity`
                    Minimum=0 Maximum=100 Width=200 CenterV />
        </HStack>
    </root>
    """)]
partial class Invert : Card, ISingleImageEffect
{
    public string DisplayName => "Invert";
    public IEnumerable<string> Keywords => ["Negative"];

    public event Action? ParametersUpdated;

    public ICanvasImage GetEffect(ICanvasImage input)
    {
        float t = (float)Intensity / 100f;

        float scale = 1 - 2 * t;
        float offset = t;

        return new ColorMatrixEffect
        {
            Source = input,
            ColorMatrix = new Matrix5x4
            {
                M11 = scale,
                M22 = scale,
                M33 = scale,
                M44 = 1,

                M51 = offset,
                M52 = offset,
                M53 = offset
            }
        };
    }

    public ICanvasImage GetExample(ICanvasImage input)
    {
        float t = 1;

        float scale = 1 - 2 * t;
        float offset = t;

        return new ColorMatrixEffect
        {
            Source = input,
            ColorMatrix = new Matrix5x4
            {
                M11 = scale,
                M22 = scale,
                M33 = scale,
                M44 = 1,

                M51 = offset,
                M52 = offset,
                M53 = offset
            }
        };
    }

    public Invert()
    {
        Init();
        IntensityProp.Watch(_ => ParametersUpdated?.Invoke());
    }
}