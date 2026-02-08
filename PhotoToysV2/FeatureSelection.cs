namespace PhotoToysV2;

[QuickMarkup("""
    string DefaultDisplayName = "None";
    ICanvasImage? CurrentDisplay;
    IImageEffect? SelectedTab;
    <root>
        <OrientedStack Spacing=16>
            <ScrollViewer
                HorizontalScrollBarVisibility=Auto
                HorizontalScrollMode=Auto
                VerticalScrollMode=Disabled
            >
                <HStack Spacing=16 XYFocusKeyboardNavigation=Enabled>
                    foreach (var x in `imageEffects`)
                        <ButtonCard
                            ExampleImage=`ExampleImage(x)`
                            Text=`x?.DisplayName ?? DefaultDisplayName`
                            IsChecked=`x == SelectedTab`
                            IsEnabled=`x != SelectedTab`
                            Click+=`(_, _) => SelectedTab = x`
                        />
                </HStack>
            </ScrollViewer>
        </OrientedStack>
    </root>
    """)]
partial class FeatureSelection : Card
{
    ICanvasImage? ExampleImage(IImageEffect effect)
    {
        return effect switch
        {
            ISingleImageEffect single =>
                CurrentDisplay is null ? null :
                single.GetExample(CurrentDisplay),
            ICreateImageEffect created => created.GetExample(),
            null => CurrentDisplay
        };
    }
    IImageEffect[] imageEffects;
    public FeatureSelection(IImageEffect[] imageEffects)
    {
        this.imageEffects = imageEffects;
        Init();
    }
    public event Action<IImageEffect?>? UpdateSelectedTab;
}
