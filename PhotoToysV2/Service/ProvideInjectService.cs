namespace PhotoToysV2.Service;

public static class ProvideInjectService
{
    // Attached property holding the local provide dictionary
    private static readonly DependencyProperty ProvideStoreProperty =
        DependencyProperty.RegisterAttached(
            "ProvideStore",
            typeof(Dictionary<string, object>),
            typeof(ProvideInjectService),
            new PropertyMetadata(null));

    private static Dictionary<string, object> GetOrCreateStore(DependencyObject obj)
    {
        var store = (Dictionary<string, object>)obj.GetValue(ProvideStoreProperty);
        if (store == null)
        {
            store = [];
            obj.SetValue(ProvideStoreProperty, store);
        }
        return store;
    }

    // ---------- PROVIDE ----------

    public static void Provide<T>(this DependencyObject uiNode, T value)
        => uiNode.Provide(typeof(T).FullName ?? typeof(T).Name, value);

    public static void Provide<T>(this DependencyObject uiNode, string key, T value)
    {
        var store = GetOrCreateStore(uiNode);
        store[key] = value!;
    }

    // ---------- INJECT ----------

    public static T Inject<T>(this DependencyObject uiNode)
        => uiNode.Inject<T>(typeof(T).FullName ?? typeof(T).Name);

    public static T Inject<T>(this DependencyObject uiNode, string key)
    {
        DependencyObject? current = uiNode;

        while (current != null)
        {
            var store = (Dictionary<string, object>)current.GetValue(ProvideStoreProperty);
            if (store != null && store.TryGetValue(key, out var value))
                return (T)value;

            current = VisualTreeHelper.GetParent(current);
        }

        throw new InvalidOperationException(
            $"No provider found for key '{key}' and type '{typeof(T)}'.");
    }
}