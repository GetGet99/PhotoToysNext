using PhotoToysV2.Service;

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
        deleteWarning=<ContentDialog
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
            <VStack Spacing=12 Margin=16 Grid_Row=1>
                <HStack CenterH>
                    <HStack Margin=`new(0,0,8,0)` IsVisible=`SelectedTab is null`>
                        <Button Padding=12 FullRounded Content=<SymbolIcon(Delete)
                            Foreground=/-
                                imageDisplay.CurrentDisplay is not null ? fillCritical.Value :
                                    new SolidColorBrush(((SolidColorBrush)fillCritical.Value).Color) { Opacity = 0.4 }
                            -/
                            Tooltip="Clear Image"
                        />
                            IsEnabled=`imageDisplay.CurrentDisplay is not null`
                            Click+=/-async (_, _) => {
                            if (await deleteWarning.ShowAsync() is ContentDialogResult.Primary) {
                                imageDisplay.Reset();
                                SelectedTab = null;
                            }
                        }-/ />
                    </HStack>
                    <HStack Spacing=8 CenterH>
                        <Button Padding=12 FullRounded Content=<SymbolIcon(Undo) />
                            Tooltip="Undo"
                            IsEnabled=`History.Current.CanUndo` Click+=/-(_, _) => {
                            imageDisplay.Undo();
                            SelectedTab = null;
                        }-/ />
                        <Button Padding=12 FullRounded Content=<SymbolIcon(Redo) />
                            Tooltip="Redo"
                            IsEnabled=`History.Current.CanRedo` Click+=/-(_, _) => {
                            imageDisplay.Redo();
                            SelectedTab = null;
                        }-/ />
                        <HStack IsVisible=`SelectedTab is not null` Spacing=8 CenterH>
                            <Border Child=`(UIElement)SelectedTab` />
                            <Button Padding=12 FullRounded Content=<SymbolIcon Symbol=Accept /> Click+=/-(_, _) => {
                                imageDisplay.Apply();
                                SelectedTab = null;
                            }-/ Tooltip="Apply changes"
                            />
                            holdBtn = <HoldButton Padding=12 FullRounded
                                Content=<SymbolIcon Symbol=View />
                                IsHolding=>`ShouldShowOriginal`
                                Tooltip="Hold to show original image\n\nHint: Hold CTRL to show original image"
                                IsEnabled=`imageDisplay.HasInputImage`
                                @Click+=`holdBtnTip.IsOpen = true`
                            />
                        </HStack>
                        <HStack IsVisible=`SelectedTab is null` Spacing=8 CenterH Margin=`new(-8,0,0,0)`>
                            <Button IsEnabled=`imageDisplay.CurrentDisplay is not null` Padding=12 FullRounded Content=<SymbolIcon Symbol=Save /> @Click+=`imageDisplay.Save()` />
                            <Button IsEnabled=`imageDisplay.CurrentDisplay is not null` Padding=12 FullRounded Content=<SymbolIcon Symbol=Copy /> @Click+=`imageDisplay.Copy()` />
                        </HStack>
                    </HStack>
                </HStack>
                <Grid>
                    <FeatureSelection(`imageEffects`) CurrentDisplay=`imageDisplay.CurrentDisplay` SelectedTab=`createMode() ? null : SelectedTab` SelectedTab=>`SelectedTab` IsVisible=`!createMode()` />
                    <FeatureSelection(`createEffects`) DefaultDisplayName="Bring your own" CurrentDisplay=`imageDisplay.CurrentDisplay` SelectedTab=`createMode() ? SelectedTab : null` SelectedTab=>`SelectedTab` IsVisible=`createMode()` />
                </Grid>
            </VStack>
            holdBtnTip=<TeachingTip
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
        holdBtnTip.Target = holdBtn;
        var oldSelected = SelectedTab;
        SelectedTabProp.Watch(x =>
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
