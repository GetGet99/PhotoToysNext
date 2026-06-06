using System.Numerics;

namespace PhotoToysV2.Effects;

[QuickMarkup("""
    double EdgeStrength = 0.5;
    double Smoothness = 4;
    double ColorLevels = 8;
    double Intensity = 100;
    <root Padding=`new(24,16,24,16)`>
        <Grid RowSpacing=16 ColumnSpacing=16
            RowDefinitions=<>
                foreach (var i in ..2) <RowDefinition Height=`Auto()` />
            </>
            ColumnDefinitions=<>
                foreach (var i in ..4) <ColumnDefinition Width=`Auto()` />
            </>
        >
            <TextBlock Text="Edge Strength" CenterV Grid.Row=0 Grid.Column=0 />
            <Slider Value<=>`EdgeStrength`
                    Minimum=0 Maximum=1.0 StepFrequency=0.05 Width=200 CenterV Grid.Row=0 Grid.Column=1 />
            <TextBlock Text="Smoothness" CenterV Grid.Row=0 Grid.Column=2 />
            <Slider Value<=>`Smoothness`
                    Minimum=0 Maximum=10 StepFrequency=0.25 Width=200 CenterV Grid.Row=0 Grid.Column=3 />
            <TextBlock Text="Color Level" CenterV Grid.Row=1 Grid.Column=0 />
            <Slider Value<=>`ColorLevels`
                    Minimum=2 Maximum=16 Width=200 CenterV Grid.Row=1 Grid.Column=1 />
            <TextBlock Text="Intensity" CenterV Grid.Row=1 Grid.Column=2 />
            <Slider Value<=>`Intensity`
                    Minimum=0 Maximum=100 Width=200 CenterV Grid.Row=1 Grid.Column=3 />
        </Grid>
    //
    </root>
    """)]
partial class Cartoon : Card, ISingleImageEffect
{
    public string DisplayName => "Cartoon";
    public IEnumerable<string> Keywords => ["Comic", "Toon"];

    public event Action? ParametersUpdated;

    ICanvasImage Posterize(ICanvasImage input, int levels)
    {
        float step = 1f / (levels - 1);

        return new DiscreteTransferEffect
        {
            Source = input,
            RedTable = BuildTable(levels),
            GreenTable = BuildTable(levels),
            BlueTable = BuildTable(levels),
            AlphaTable = new float[] { 0, 1 }
        };
    }

    float[] BuildTable(int levels)
    {
        var table = new float[levels];
        for (int i = 0; i < levels; i++)
            table[i] = i / (float)(levels - 1);
        return table;
    }
    ICanvasImage NeutralizeEdges(ICanvasImage edges)
    {
        return new ColorMatrixEffect
        {
            Source = edges,
            ColorMatrix = new Matrix5x4
            {
                M11 = 0.299f,
                M21 = 0.299f,
                M31 = 0.299f,
                M12 = 0.587f,
                M22 = 0.587f,
                M32 = 0.587f,
                M13 = 0.114f,
                M23 = 0.114f,
                M33 = 0.114f,
                M44 = 1
            }
        };
    }
    ICanvasImage AmplifyEdges(ICanvasImage edges, float gain)
    {
        return new ColorMatrixEffect
        {
            Source = edges,
            ColorMatrix = new Matrix5x4
            {
                M11 = gain,
                M22 = gain,
                M33 = gain,
                M44 = 1
            }
        };
    }


    public ICanvasImage GetEffect(ICanvasImage input)
    {
        var edges = new EdgeDetectionEffect
        {
            Source = input,
            Amount = (float)EdgeStrength,
            BlurAmount = 0.5f
        };

        var smooth = new GaussianBlurEffect
        {
            Source = input,
            BlurAmount = (float)Smoothness
        };

        var posterized = Posterize(smooth, (int)ColorLevels);

        var neutral = NeutralizeEdges(edges);
        var amplified = AmplifyEdges(neutral, 2.0f);

        var ink = new AlphaMaskEffect
        {
            Source = new ColorSourceEffect { Color = Colors.Black },
            AlphaMask = new LuminanceToAlphaEffect { Source = amplified }
        };

        var cartoon = new CompositeEffect
        {
            Mode = CanvasComposite.SourceOver,
            Sources =
            {
                posterized,
                ink
            }
        };


        var final = new ArithmeticCompositeEffect
        {
            Source1 = cartoon,
            Source2 = input,
            Source1Amount = (float)(Intensity / 100),
            Source2Amount = 1 - (float)(Intensity / 100)
        };

        var bounds = final.Bounds;

        return new Transform2DEffect
        {
            Source = final,
            TransformMatrix = Matrix3x2.CreateTranslation(-(float)bounds.Left, -(float)bounds.Top)
        };

    }

    public Cartoon() : this(false) { }
    private Cartoon(bool isExample)
    {
        if (!isExample)
            Init();
        Effect(() => ParametersUpdated?.Invoke(),
            EdgeStrengthProp,
            SmoothnessProp,
            IntensityProp,
            ColorLevelsProp);
    }
    static Cartoon ExampleCartoon { get; } = new(true);

    public ICanvasImage GetExample(ICanvasImage input) => ExampleCartoon.GetEffect(input);
}