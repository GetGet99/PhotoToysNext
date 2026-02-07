using System.Numerics;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double BlurAmount = 0;
    <root Toolbar>
        <HStack Spacing=16>
            <TextBlock Text="Intensity" CenterV />
            <Slider Value=3 Value=>/-BlurAmount-/ Minimum=0 Maximum=10 StepFrequency=0.01 Width=200 CenterV />
        </HStack>
    </root>
    """)]
partial class Blur : Card, ISingleImageEffect
{
    public string DisplayName => "Blur";

    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;
    public ICanvasImage GetEffect(ICanvasImage input)
    {
        var blur = new GaussianBlurEffect
        {
            Source = input,
            BorderMode = EffectBorderMode.Soft,
            BlurAmount = (float)BlurAmount,
        };
        var bounds = blur.Bounds;

        return new Transform2DEffect
        {
            Source = blur,
            TransformMatrix = Matrix3x2.CreateTranslation(-(float)bounds.Left, -(float)bounds.Top)
        };
    }

    public Blur()
    {
        Init();
        BlurAmountProp.Watch(_ => ParametersUpdated?.Invoke());
    }
    public ICanvasImage GetExample(ICanvasImage input)
    {
        var blur = new GaussianBlurEffect
        {
            Source = input,
            BorderMode = EffectBorderMode.Soft,
            BlurAmount = 3f,
        };

        return blur;
    }
}
