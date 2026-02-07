using Windows.UI.Xaml.Input;

namespace PhotoToysV2.Controls;

[QuickMarkup("""
    private bool IsHoldingWithMouse;
    private bool IsHoldingWithKeyboard;
    bool IsHolding => `IsEnabled && (IsHoldingWithMouse || IsHoldingWithKeyboard)`;
    bool IsEnabled;
    <setup>
    var defaultState = ThemeResources.Get<Brush>("ControlFillColorDefaultBrush", this).CreateReadOnlyReference();
    var defaultStateDisabled = ThemeResources.Get<Brush>("ControlFillColorDisabledBrush", this).CreateReadOnlyReference();
    var defaultStateForeground = ThemeResources.Get<Brush>("TextFillColorPrimaryBrush", this).CreateReadOnlyReference();
    var defaultStateForegroundDisabled = ThemeResources.Get<Brush>("TextFillColorDisabledBrush", this).CreateReadOnlyReference();
    var holdingState = ThemeResources.Get<Brush>("AccentFillColorTertiaryBrush", this).CreateReadOnlyReference();
    var holdingStateForeground = ThemeResources.Get<Brush>("TextOnAccentFillColorPrimaryBrush", this).CreateReadOnlyReference();
    Color C(Brush b) => ((SolidColorBrush)b).Color;
    </setup>
    <root
        BaseIsEnabled=`IsEnabled && !IsHoldingWithKeyboard`
        Background=`IsHolding ? holdingState.Value : defaultState.Value`
        Foreground=`IsHolding ? holdingStateForeground.Value : defaultStateForeground.Value`
        `Resources["ButtonBackgroundPressed"]`=`holdingState.Value`
        `Resources["ButtonForegroundPressed"]`=`holdingStateForeground.Value`
        `disabledBG.Color`=`C(IsHolding ? holdingState.Value : defaultStateDisabled.Value)`
        `disabledFG.Color`=`C(IsHolding ? holdingStateForeground.Value : defaultStateForegroundDisabled.Value)`
        `Resources["ButtonBackgroundDisabled"]`=`disabledBG`
        `Resources["ButtonForegroundDisabled"]`=`disabledFG`
    />
    """)]
partial class HoldButton : Button
{
    SolidColorBrush disabledBG = new(), disabledFG = new();
    public new event Action Click;
    bool BaseIsEnabled
    {
        get => base.IsEnabled;
        set => base.IsEnabled = value;
    }
    public HoldButton()
    {
        Init();

        var cw = CoreWindow.GetForCurrentThread();
        cw.KeyDown += Cw_KeyChanged;
        cw.KeyUp += Cw_KeyChanged;
    }
    DateTime pressed;
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        pressed = DateTime.Now;
        CapturePointer(e.Pointer);
        IsHoldingWithMouse = true;
    }
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        IsHoldingWithMouse = false;
        var diff = DateTime.Now - pressed;
        if (diff < TimeSpan.FromMilliseconds(200))
        {
            Click?.Invoke();
        }
    }

    private void Cw_KeyChanged(CoreWindow sender, KeyEventArgs args)
    {
        if (sender.GetAsyncKeyState(Windows.System.VirtualKey.Control) is not CoreVirtualKeyStates.None)
        {
            IsHoldingWithKeyboard = true;
        }
        else
        {
            IsHoldingWithKeyboard = false;
        }
    }
}