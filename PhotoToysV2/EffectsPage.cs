namespace PhotoToysV2;

[QuickMarkup("""
    using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
    private IImageEffect? SelectedTab;
    private bool ShouldShowOriginal;
    <setup>
    bool createMode() => imageDisplay?.CurrentDisplay is null;
    var fillCritical = ThemeResources.Get<Brush>("SystemFillColorCriticalBrush", this).CreateReadOnlyReference();
    </setup>
    <root
        `deleteWarning`=<ContentDialog
            PrimaryButtonText="Yes"
            CloseButtonText="No"
            Title="Delete this image?"
            Content="All unsaved changes to your current image will be lost!"
        />
    >
        <Grid>
            <.RowDefinitions>
                <RowDefinition />
                <RowDefinition Auto />
            </.RowDefinitions>
            imageDisplay = <SingleImageDisplay ImageEffect=`SelectedTab` ShouldAskToOpen=`!createEffectMode` ShouldShowOriginal=`ShouldShowOriginal` />
            <VStack Spacing=12 Margin=16 Grid.Row=1>
                <HStack CenterH Spacing=8>
                    if (`SelectedTab is null`) {
                        <Button Padding=12 FullRounded
                            Content=<SymbolIcon(Delete)
                                Foreground=`
                                    imageDisplay.CurrentDisplay is not null ? fillCritical.Value :
                                        new SolidColorBrush(((SolidColorBrush)fillCritical.Value).Color) { Opacity = 0.4 }`
                                ToolTipService.ToolTip="Clear Image"
                            />
                            IsEnabled=`imageDisplay.CurrentDisplay is not null`
                            Click+=`async (_, _) => {
                            if (await deleteWarning.ShowAsync() is ContentDialogResult.Primary) {
                                imageDisplay.Reset();
                                SelectedTab = null;
                            }
                        }` />
                    }
                    <Button Padding=12 FullRounded Content=<SymbolIcon(Undo) />
                        ToolTipService.ToolTip="Undo"
                        IsEnabled=`History.Current.CanUndo` Click+=`(_, _) => {
                        imageDisplay.Undo();
                        SelectedTab = null;
                    }` />
                    <Button Padding=12 FullRounded Content=<SymbolIcon(Redo) />
                        ToolTipService.ToolTip="Redo"
                        IsEnabled=`History.Current.CanRedo` Click+=`(_, _) => {
                        imageDisplay.Redo();
                        SelectedTab = null;
                    }` />
                    if (`SelectedTab is not null`) {
                        <Border Child=`(UIElement)SelectedTab` />
                        <Button Padding=12 FullRounded Content=<SymbolIcon Symbol=Accept /> Click+=`(_, _) => {
                            imageDisplay.Apply();
                            SelectedTab = null;
                        }` ToolTipService.ToolTip="Apply changes"
                        />
                        ref holdBtn = <HoldButton Padding=12 FullRounded
                            Content=<SymbolIcon Symbol=View />
                            IsHolding=>`ShouldShowOriginal`
                            ToolTipService.ToolTip="Hold to show original image\n\nHint: Hold CTRL to show original image"
                            IsEnabled=`imageDisplay.HasInputImage`
                            @Click+=`holdBtnTip.IsOpen = true`
                        />
                    } else {
                        <Button IsEnabled=`imageDisplay.CurrentDisplay is not null` Padding=12 FullRounded Content=<SymbolIcon Symbol=Save /> @Click+=`imageDisplay.Save()` />
                        <Button IsEnabled=`imageDisplay.CurrentDisplay is not null` Padding=12 FullRounded Content=<SymbolIcon Symbol=Copy /> @Click+=`imageDisplay.Copy()` />
                    }
                </HStack>
                <Grid>
                    if (`!createMode()`)
                        <FeatureSelection(`imageEffects`) CurrentDisplay=`imageDisplay.CurrentDisplay` SelectedTab=`createMode() ? null : SelectedTab` SelectedTab=>`SelectedTab` />
                    else
                        <FeatureSelection(`createEffects`) DefaultDisplayName="Bring your own" CurrentDisplay=`imageDisplay.CurrentDisplay` SelectedTab=`createMode() ? SelectedTab : null` SelectedTab=>`SelectedTab` />
                </Grid>
            </VStack>
            holdBtnTip=<TeachingTip
                Target=`holdBtn`
                IconSource=<SymbolIconSource Symbol=`(Symbol)0xE815` />
                Title="Hold Button"
                Subtitle="Hold to show the original image"
            />
        </Grid>
    </root>
    """)]
public partial class EffectsPage : Page
{
    //TeachingTip holdBtnTip = null!;
    ContentDialog deleteWarning = null!;
    static readonly IImageEffect?[] imageEffects = [
        null,
        new AdjustColor(),
        new Blur(),
        new BorderEf(),
        new CropEf(),
        new Grayscale(),
        new Invert(),
        new Sepia(),
        new Cartoon(),
        new Channels(),
    ];
    static readonly IImageEffect?[] createEffects = [
        null,
        new RectangleEffect(),
        new EllipseEffect(),
        new TextEffect()
    ];
    bool createEffectMode;
    public EffectsPage(bool createEffect)
    {
        createEffectMode = createEffect;
        Init();
        var oldSelected = SelectedTab;
        SelectedTabProp!.Watch(x =>
        {
            if (oldSelected is IImageEffectSelectNotify notifyDeselect)
            {
                notifyDeselect.Deselected();
            }
            if (x is IImageEffectSelectNotify notifySelect)
            {
                notifySelect.Selected();
            }
            oldSelected = x;
        });
    }
}
