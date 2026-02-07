namespace PhotoToysV2.Effects;

[QuickMarkup("""
    private bool NumberBoxMode = false;
    private bool TransposeInput = true;
    // make it so big for numberbox mode
    private double Min => /-NumberBoxMode ? -255.0 : 0.0-/;
    private double Max => /-NumberBoxMode ? 255.0 : 1.0-/;
    private Matrix5x4 MatrixFinal => /-new Matrix5x4
    {
        M11 = (float)MatrixRef[0, 0].Value,
        M12 = (float)MatrixRef[0, 1].Value,
        M13 = (float)MatrixRef[0, 2].Value,
        M14 = (float)MatrixRef[0, 3].Value,
        M21 = (float)MatrixRef[1, 0].Value,
        M22 = (float)MatrixRef[1, 1].Value,
        M23 = (float)MatrixRef[1, 2].Value,
        M24 = (float)MatrixRef[1, 3].Value,
        M31 = (float)MatrixRef[2, 0].Value,
        M32 = (float)MatrixRef[2, 1].Value,
        M33 = (float)MatrixRef[2, 2].Value,
        M34 = (float)MatrixRef[2, 3].Value,
        M41 = (float)MatrixRef[3, 0].Value,
        M42 = (float)MatrixRef[3, 1].Value,
        M43 = (float)MatrixRef[3, 2].Value,
        M44 = (float)MatrixRef[3, 3].Value,
        M51 = (float)MatrixRef[4, 0].Value,
        M52 = (float)MatrixRef[4, 1].Value,
        M53 = (float)MatrixRef[4, 2].Value,
        M54 = (float)MatrixRef[4, 3].Value,
    }-/;
    <setup>
    string[] texts = ["Red", "Green", "Blue", "Alpha", "Constant"];
    </setup>
    <root Padding=/-new(24,16,24,16)-/ >
        <Grid CenterH RowSpacing=16 ColumnSpacing=16
            RowDefinitions=<>
                for (i in ..7) <RowDefinition Height=/-Auto()-/ />
            </>
            ColumnDefinitions=<>
                for (i in ..6) <ColumnDefinition Width=/-Auto()-/ />
            </>
            /-x => {
                TransposeInputProp.Watch(transpose => {
                    if (transpose) {
                        if (x.ColumnDefinitions.Count is not 6) {
                            x.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
                        }
                    } else {
                        if (x.ColumnDefinitions.Count is 6) {
                            x.ColumnDefinitions.RemoveAt(5);
                        }
                    }
                }, immediete: true);
            }-/
        >
            <TextBlock Text=/-TransposeInput ? "Output" : "Input"-/ CenterV Grid_Row=0 Grid_Column=0 />
            for (i in 0..4) {
                <TextBlock Text=/-$"Output {texts[i]}"-/ CenterV Grid_Row=/-TransposeInput ? i + 1 : 0-/ Grid_Column=/-TransposeInput ? 0 : i + 1-/ />
            }
            for (i in 0..5) {
                <TextBlock Text=/-$"Input {texts[i]}"-/ CenterV Grid_Row=/-TransposeInput ? 0 : i + 1-/ Grid_Column=/-TransposeInput ? i + 1 : 0-/ />
                for (j in 0..4) {
                    <NumberInput Value=/-MatrixRef[i, j].Value-/ Value=>/-MatrixRef[i, j].Value-/
                        Minimum=/-Min-/ Maximum=/-Max-/ Step=0.01 Width=100
                        CenterV
                        Grid_Row=/-TransposeInput ? j + 1 : i + 1-/
                        Grid_Column=/-TransposeInput ? i + 1 : j + 1-/
                        NumberBoxMode=/-NumberBoxMode-/
                    />
                }
            }
            <HStack Grid_Row=6 Grid_ColumnSpan=6 Spacing=16 CenterH>
                <ToggleSwitch IsOn IsOn=>/-TransposeInput-/ OnContent="Output On Left" OffContent="Output On Top" />
                <ToggleSwitch IsOn=>/-NumberBoxMode-/ OnContent="Advanced Mode" OffContent="Advanced Mode" />
            </HStack>
        </Grid>
    //
    </root>
    """)]
partial class Channels : Card, ISingleImageEffect
{
    public string DisplayName => "Channels";
    public IEnumerable<string> Keywords => ["Matrix"];
    Reference<double>[,] MatrixRef;

    public event Action? ParametersUpdated;

    public ICanvasImage GetEffect(ICanvasImage input)
    {
        return new ColorMatrixEffect
        {
            Source = input,
            ColorMatrix = MatrixFinal
        };
    }

    public Channels()
    {
        MatrixRef = new Reference<double>[5, 4];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                MatrixRef[i, j] = new(i == j ? 1 : 0);
            }
        }
        Init();
        Effect(() => ParametersUpdated?.Invoke(), MatrixFinalComp);
    }
    public ICanvasImage GetExample(ICanvasImage input)
    {
        return new ColorMatrixEffect
        {
            Source = input,
            ColorMatrix = new()
            {
                M21 = 1,
                M32 = 1,
                M13 = 1,
                M14 = 1,
            }
        };
    }
}