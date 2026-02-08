using Get.Symbols;
using Windows.ApplicationModel.Core;
namespace PhotoToysV2;

[QuickMarkup("""
    using Windows.UI.Xaml.Media.Imaging;
    using CommunityToolkit.WinUI.Controls;
    using SymbolExIcon = Get.Symbols.SymbolExIcon;
    int SelectedMenuIndex = 0;
    private double TitleBarHeight = `double.NaN`;
    private double LeftInset = `double.NaN`;
    private double RightInset = `double.NaN`;
    <setup>
    var segmentedStyle = (Style)App.Current.Resources["PivotSegmentedStyle"];
    var CaptionTextBlockStyle = (Style)App.Current.Resources["CaptionTextBlockStyle"];
    </setup>
    <root>
        <Grid RowDefinitions=<>
                <RowDefinition Auto />
                <RowDefinition />
            </>
        >
            <Grid>
                titleBar = <Grid Background=`Solid(Colors.Transparent)` />
                child = <HStack Spacing=6 Height=`TitleBarHeight` Margin=`new(Math.Max(8, LeftInset),0,RightInset,0)` !IsHitTestVisible>
                    <Image Width=16 Height=16 CenterV Source=`new BitmapImage(new Uri("ms-appx:///Assets/PhotoToys.png"))` />
                    <HStack Spacing=6 CenterV>
                        <TextBlock Text="PhotoToys Next" Style=`CaptionTextBlockStyle` />
                        <TextBlock Text="ALPHA" FontSize=10 Bottom Foreground=`Solid(Colors.DarkGray)` />
                    </HStack>
                    /*
                    <Segmented Style=`segmentedStyle` CenterV Background=`Solid(Colors.Transparent)`
                        SelectedIndex=0 SelectedIndex=>`SelectedMenuIndex`>
                        foreach (var x in `Menus`) {
                            <SegmentedItem Content=`x.Name` Icon=<SymbolExIcon SymbolEx=`x.Icon` /> />
                        }
                    </Segmented>
                    */
                </HStack>
            </Grid>
            <Border Child=`Menus[SelectedMenuIndex].UI` Grid_Row=1 />
        </Grid>
    </root>
    """)]
partial class MainPage : Page
{
    (string Name, SymbolEx Icon, UIElement UI)[] Menus => field ??= [
        ("Effects", SymbolEx.Color, new EffectsPage(false)),
        ("Create", SymbolEx.ExploreContentSingle, new EffectsPage(true)),
        ("Advanced", SymbolEx.Settings, new EffectsPage(false)),
    ];
    public MainPage()
    {
        BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        TitleBarHeight = coreTitleBar.Height;
        coreTitleBar.LayoutMetricsChanged += delegate
        {
            TitleBarHeight = coreTitleBar.Height;
            LeftInset = coreTitleBar.SystemOverlayLeftInset;
            RightInset = coreTitleBar.SystemOverlayRightInset;
        };
        coreTitleBar.ExtendViewIntoTitleBar = true;
        ReactiveScheduler.AddTickCallbackForCurrentThread(delegate
        {
            _ = Dispatcher.TryRunAsync(CoreDispatcherPriority.High, ReactiveScheduler.Tick);
        });
        void InitWithPostProcess()
        {
            ((Border)VisualTreeHelper.GetParent(Menus[SelectedMenuIndex].UI))?.Child = null;
            Menus[0] = (Menus[0].Name, Menus[0].Icon, new EffectsPage(false));
            Init();
            Window.Current.SetTitleBar(titleBar);
        }
        InitWithPostProcess();
    }
}
