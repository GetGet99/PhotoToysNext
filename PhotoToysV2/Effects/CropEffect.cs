using Get.Data.XACL;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    private Rect CropRectangle;
    <root Toolbar
        cropResizer = <CropResizer Value=/-CropRectangle-/ Value=>/-CropRectangle-/ />
    >
        <HStack Spacing=16>
            <TextBlock Text="X:" CenterV />
            <NumberBox Value=/-Math.Round(X, 2)-/ Value=>/-X-/ />
            <TextBlock Text="Y:" CenterV />
            <NumberBox Value=/-Math.Round(Y, 2)-/ Value=>/-Y-/ />
            <TextBlock Text="Width:" CenterV />
            <NumberBox Value=/-Math.Round(Width, 2)-/ Value=>/-Width-/ />
            <TextBlock Text="Height:" CenterV />
            <NumberBox Value=/-Math.Round(Height, 2)-/ Value=>/-Height-/ />
        </HStack>
    </root>
    """)]
partial class CropEf : Card, ISingleImageEffectPreview, IImageEffectSelectNotify, IImageEffectApplyNotify
{
    double X { get => CropRectangle.X; set => CropRectangle = CropRectangle with { X = value }; }
    double Y { get => CropRectangle.Y; set => CropRectangle = CropRectangle with { Y = value }; }
    new double Width { get => CropRectangle.Width; set => CropRectangle = CropRectangle with { Width = value }; }
    new double Height { get => CropRectangle.Height; set => CropRectangle = CropRectangle with { Height = value }; }
    public string DisplayName => "Crop";
    private CropResizer cropResizer = null!;

    public IEnumerable<string> Keywords => [];

    public event Action? ParametersUpdated;
    // while editing, do not change input image
    public ICanvasImage GetPreview(ICanvasImage input)
    {

        return new CompositeEffect
        {
            Sources =
            {
                new CropEffect()
                {
                    Source = input,
                    SourceRectangle = CropRectangle
                },
                new ColorMatrixEffect
                {
                    Source = input,
                    ColorMatrix = new Matrix5x4(
                        1, 0, 0, 0,
                        0, 1, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 0.5f,
                        0, 0, 0, 0
                    )
                }
            }
        };
    }
    public ICanvasImage GetEffect(ICanvasImage input)
    {
        return new AtlasEffect()
        {
            Source = input,
            SourceRectangle = CropRectangle,
            CacheOutput = true
        };
    }

    public CropEf()
    {
        Init();
        CropRectangleProp.Watch(_ => ParametersUpdated?.Invoke());
    }
    public ICanvasImage GetExample(ICanvasImage input)
    {
        var bounds = input.Bounds;
        var x = bounds.Width / 4;
        var y = bounds.Height / 4;
        return new AtlasEffect()
        {
            Source = input,
            SourceRectangle = new(x, y, x * 2, y * 2)
        };
    }

    public void Selected()
    {
        ResizerService.Instance.Resizer = cropResizer;
    }

    public void Deselected()
    {
        if (ResizerService.Instance.Resizer == cropResizer)
            ResizerService.Instance.Resizer = null;
    }

    public void Applied()
    {
        cropResizer.Reset();
    }
}
