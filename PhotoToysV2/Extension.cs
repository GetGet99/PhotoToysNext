using Get.Data.Bindings;
using Get.Data.Properties;

namespace PhotoToysV2;

static class Extension
{
    extension<T>(IReadOnlyProperty<T> item)
    {
        public void ApplyAndRegisterForNewValue(Action<T> action)
        {
            item.ApplyAndRegisterForNewValue((x, _) => action(x));
        }
    }
    extension<T>(T item)
    {
        public T WithCustomCode(Action<T> action)
        {
            action(item);
            return item;
        }
    }

    public static Reference<T> CreateReadOnlyRefrence<T>(this DependencyObject element, DependencyProperty property)
    {
        var r = Ref((T)element.GetValue(property));
        void SetValue()
        {
            r.Value = (T)element.GetValue(property);
        }
        element.RegisterPropertyChangedCallback(property, (_, _) => SetValue());
        return r;
    }
    extension<T>(T element) where T : DependencyObject
    {
        public void BindReferenceOneWayToSource<T2>(DependencyProperty property, Reference<T2> reference)
        {
            void SetValue()
            {
                reference.Value = (T2)element.GetValue(property);
            }
            SetValue();
            element.RegisterPropertyChangedCallback(property, (_, _) => SetValue());
        }
        public void BindReferenceOneWayToSource<T2, T3>(DependencyProperty property, Reference<T2> reference, Func<T3, T2> converter)
        {
            void SetValue()
            {
                reference.Value = converter((T3)element.GetValue(property));
            }
            SetValue();
            element.RegisterPropertyChangedCallback(property, (_, _) => SetValue());
        }
    }

    extension<T>(T element) where T : UIElement
    {
        public bool IsVisible
        {
            get => element.Visibility is Visibility.Visible;
            set => element.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    extension<T>(T element) where T : FrameworkElement
    {
        public CoreCursorType Cursor
        {
            get => FrameworkElementExtensions.GetCursor(element);
            set => FrameworkElementExtensions.SetCursor(element, value);
        }
        public T Center()
        {
            element.HorizontalAlignment = HorizontalAlignment.Center;
            element.VerticalAlignment = VerticalAlignment.Center;
            return element;
        }
        public T CenterH()
        {
            element.HorizontalAlignment = HorizontalAlignment.Center;
            return element;
        }
        public T CenterV()
        {
            element.VerticalAlignment = VerticalAlignment.Center;
            return element;
        }
        public T Left()
        {
            element.HorizontalAlignment = HorizontalAlignment.Left;
            return element;
        }
        public T Top()
        {
            element.VerticalAlignment = VerticalAlignment.Top;
            return element;
        }
        public T Bottom()
        {
            element.VerticalAlignment = VerticalAlignment.Bottom;
            return element;
        }
        public T Right()
        {
            element.HorizontalAlignment = HorizontalAlignment.Right;
            return element;
        }
    }
    extension(Border element)
    {
        public Border FullRounded()
        {
            element.SizeChanged += FullRoundedSizeChangedHandler;
            FullRoundedSizeChangedHandler(element, null);
            return element;
        }
    }
    extension(RowDefinition rd)
    {
        public void Auto()
        {
            rd.Height = GridLength.Auto;
        }
    }
    extension(ColumnDefinition rd)
    {
        public void Auto()
        {
            rd.Width = GridLength.Auto;
        }
    }
    extension<T>(T element) where T : Control
    {
        public T FullRounded()
        {
            element.SizeChanged += FullRoundedSizeChangedHandler;
            FullRoundedSizeChangedHandler(element, null);
            return element;
        }
    }
    private static void FullRoundedSizeChangedHandler(object sender, SizeChangedEventArgs? e)
    {
        if (sender is FrameworkElement ele)
        {
            double radius;
            if (e is not null)
                radius = Math.Min(e.NewSize.Width, e.NewSize.Height) / 2;
            else
                radius = Math.Min(ele.ActualWidth, ele.ActualHeight) / 2;
            if (ele is Control control)
            {
                control.CornerRadius = new(radius);
            }
            else if (ele is Border border)
            {
                border.CornerRadius = new(radius);
            }
        }
    }
    extension<T>(IReadOnlyBinding<T> prop)
    {
        public Reference<T> CreateReadOnlyReference()
        {
            var r = new Reference<T>(prop.CurrentValue);
            prop.ValueChanged += (_, val) => r.Value = val;
            return r;
        }
    }
    public static void FirstLoadedEv(this FrameworkElement element, Action ev)
    {
        element.Loaded += Element_Loaded;
        void Element_Loaded(object sender, RoutedEventArgs e)
        {
            element.Loaded -= Element_Loaded;
            ev();
        }
    }
    static CanvasDevice sharedDevice = CanvasDevice.GetSharedDevice();
    extension(ICanvasImage? canvasImage)
    {
        public Rect Bounds
        {
            get
            {
                if (canvasImage is null)
                    return default;
                if (canvasImage is AtlasEffect atlas)
                    return new(0, 0, atlas.SourceRectangle.Width, atlas.SourceRectangle.Height);
                return canvasImage.GetBounds(sharedDevice);
            }
        }
    }
}