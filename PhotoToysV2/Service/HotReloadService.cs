namespace PhotoToysV2.Service;

static class HotReloadService
{
    static Action? _OnHotReload;
    public static event Action OnHotReload
    {
        add
        {
#if DEBUG
            if (_OnHotReload is null && value is not null)
            {
                CoreWindow.GetForCurrentThread().KeyDown += HotReloadService_KeyDown;
            }
            _OnHotReload += value;
#endif
        }
        remove
        {
#if DEBUG
            _OnHotReload -= value;
            if (_OnHotReload is null)
            {
                CoreWindow.GetForCurrentThread().KeyDown -= HotReloadService_KeyDown;
            }
#endif
        }
    }
#if DEBUG
    private static void HotReloadService_KeyDown(CoreWindow sender, KeyEventArgs args)
    {
        if (args.VirtualKey is Windows.System.VirtualKey.R && CoreWindow.GetForCurrentThread().GetAsyncKeyState(Windows.System.VirtualKey.Control) is CoreVirtualKeyStates.Down)
        {
            _OnHotReload?.Invoke();
        }
    }
#endif
}
