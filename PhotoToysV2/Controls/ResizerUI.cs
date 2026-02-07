using Windows.UI.Xaml.Input;

namespace PhotoToysV2.Controls;

[QuickMarkup("""
    double ParentZoomFactor = 1;
    Size InitialSize;
    private double UserLeft = 0;
    private double UserTop = 0;
    private double UserWidth = /-double.NaN-/;
    private double UserHeight = /-double.NaN-/;
    private double CurrentWidth => /-double.IsNaN(UserWidth) ? InitialSize.Width : UserWidth-/;
    private double CurrentHeight => /-double.IsNaN(UserHeight) ? InitialSize.Height : UserHeight-/;
    private double UIScale => /- Math.Max(1 / ParentZoomFactor, 0.4f) -/;
    private double ResizerSize => /- 10 * UIScale -/;
    private double BorderSize => /- 3 * UIScale -/;
    private double Margin => /- ResizerSize - BorderSize -/;
    private Thickness mg => /- new(-Margin / 2) -/;
    <root
        Canvas_Left=/-internalLayoutChanges ? UserLeft : 0-/
        Canvas_Top=/-internalLayoutChanges ? UserTop : 0-/
        Width=/-internalLayoutChanges ? CurrentWidth : InitialSize.Width-/
        Height=/-internalLayoutChanges ? CurrentHeight : InitialSize.Height-/
    >
        // Sides
        _l = <Border Width=/-BorderSize-/ Left />
        _r = <Border Width=/-BorderSize-/ Right />
        _t = <Border Height=/-BorderSize-/ Top />
        _b = <Border Height=/-BorderSize-/ Bottom />
        // Middle Handle
        l = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ CenterV Left />
        r = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ CenterV Right />
        t = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ Top CenterH />
        b = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ Bottom CenterH />
        // Corners
        tl = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ Top Left />
        tr = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ Top Right/>
        bl = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ Bottom Left />
        br = <Border Width=/-ResizerSize-/ Height=/-ResizerSize-/ Margin=/-mg-/ Bottom Right />
    </root>
    """)]
abstract partial class ResizerUI : Grid
{
    protected Rect CurrentRect
    {
        get
        {
            return new(UserLeft, UserTop, CurrentWidth, CurrentHeight);
        }
        set
        {
            if (value == default)
            {
                UserLeft = UserTop = 0;
                UserWidth = UserHeight = double.NaN;
            } else
            {
                UserLeft = value.Left;
                UserTop = value.Top;
                UserWidth = value.Width;
                UserHeight = value.Height;
            }
        }
    }
    protected void ResetAll()
    {
        UserLeft = UserTop = 0;
        UserWidth = UserHeight = double.NaN;
    }
    ResizerMode resizerMode;
    bool internalLayoutChanges;
    bool sizeFromCenterMode;
    public ResizerUI(ResizerMode resizerMode, bool internalLayoutChanges, bool sizeFromCenterMode = false)
    {
        this.resizerMode = resizerMode;
        this.internalLayoutChanges = internalLayoutChanges;
        this.sizeFromCenterMode = sizeFromCenterMode;
        Init();
        InitialSizeProp.Watch(_ => RaiseOnChange());
        var accentBrush = ThemeResources.Get<Brush>("AccentFillColorDefaultBrush", this);
        accentBrush.ApplyAndRegisterForNewValue(brush =>
        {
            _t.Background = _b.Background = _l.Background = _r.Background =
            t.Background = b.Background = l.Background = r.Background =
            tl.Background = tr.Background = bl.Background = br.Background = brush;
        });
        l.ManipulationMode = r.ManipulationMode = ManipulationModes.TranslateX;
        t.ManipulationMode = b.ManipulationMode = ManipulationModes.TranslateY;
        _l.ManipulationMode = _r.ManipulationMode = ManipulationModes.TranslateX;
        _t.ManipulationMode = _b.ManipulationMode = ManipulationModes.TranslateY;
        tl.ManipulationMode = tr.ManipulationMode = bl.ManipulationMode = br.ManipulationMode
            = ManipulationModes.TranslateX | ManipulationModes.TranslateY;

        //void UpdateResizerVisibility()
        //{
        //    if (double.IsNaN(Height))
        //    {
        //        t.Visibility = Visibility.Collapsed;
        //        b.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        t.Visibility = Visibility.Visible;
        //        b.Visibility = Visibility.Visible;
        //    }
        //    if (double.IsNaN(Width))
        //    {
        //        l.Visibility = Visibility.Collapsed;
        //        r.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        l.Visibility = Visibility.Visible;
        //        r.Visibility = Visibility.Visible;
        //    }
        //    if (double.IsNaN(Height) || double.IsNaN(Width))
        //    {
        //        tl.Visibility = tr.Visibility = bl.Visibility = br.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        tl.Visibility = tr.Visibility = bl.Visibility = br.Visibility = Visibility.Visible;
        //    }
        //}
        //UpdateResizerVisibility();

        double left = 0, top = 0, width = 0, height = 0;
        bool isAuto = false;
        foreach (var t in (Border[])[t, _t])
        {
            t.Cursor = CoreCursorType.SizeNorthSouth;
            t.ManipulationStarted += (_, e) =>
            {
                e.Handled = true;
                top = Canvas.GetTop(this);
                height = CurrentHeight;
                isAuto = double.IsNaN(height);
            };
            t.ManipulationDelta += (_, e) =>
            {
                e.Handled = true;
                var y = e.Cumulative.Translation.Y / ParentZoomFactor;
                if (isAuto)
                {
                    Canvas.SetTop(this, top + y);
                    return;
                }
                TTranslate(y);
                RaiseOnChange();
            };
        }
        foreach (var b in (Border[])[b, _b])
        {
            b.Cursor = CoreCursorType.SizeNorthSouth;
            b.ManipulationStarted += (_, e) =>
            {
                e.Handled = true;
                top = Canvas.GetTop(this);
                height = CurrentHeight;
                isAuto = double.IsNaN(height);
            };
            b.ManipulationDelta += (_, e) =>
            {
                e.Handled = true;
                if (isAuto)
                    return;
                var y = e.Cumulative.Translation.Y / ParentZoomFactor;
                BTranslate(y);
                RaiseOnChange();
            };
        }
        foreach (var l in (Border[])[l, _l])
        {
            l.Cursor = CoreCursorType.SizeWestEast;
            l.ManipulationStarted += (_, e) =>
            {
                e.Handled = true;
                left = Canvas.GetLeft(this);
                width = CurrentWidth;
                isAuto = double.IsNaN(width);
            };
            l.ManipulationDelta += (_, e) =>
            {
                e.Handled = true;
                var x = e.Cumulative.Translation.X / ParentZoomFactor;
                if (isAuto)
                {
                    Canvas.SetLeft(this, left + x);
                    return;
                }
                LTranslate(x);
                RaiseOnChange();
            };
        }
        foreach (var r in (Border[])[r, _r])
        {
            r.Cursor = CoreCursorType.SizeWestEast;
            r.ManipulationStarted += (_, e) =>
            {
                e.Handled = true;
                left = Canvas.GetLeft(this);
                width = CurrentWidth;
                isAuto = double.IsNaN(width);
            };
            r.ManipulationDelta += (_, e) =>
            {
                e.Handled = true;
                if (isAuto)
                    return;
                var x = e.Cumulative.Translation.X / ParentZoomFactor;
                RTranslate(x);
                RaiseOnChange();
            };
        }

        foreach (var corner in (Border[])[tl, tr, bl, br])
        {
            corner.Cursor = corner == tl || corner == br ? CoreCursorType.SizeNorthwestSoutheast : CoreCursorType.SizeNortheastSouthwest;
            corner.ManipulationStarted += (_, e) =>
            {
                e.Handled = true;
                top = Canvas.GetTop(this);
                left = Canvas.GetLeft(this);
                width = CurrentWidth;
                height = CurrentHeight;
                isAuto = double.IsNaN(width);
            };
        }
        tl.ManipulationDelta += (_, e) =>
        {
            e.Handled = true;
            if (isAuto)
                return;
            var x = e.Cumulative.Translation.X / ParentZoomFactor;
            var y = e.Cumulative.Translation.Y / ParentZoomFactor;
            TTranslate(y);
            LTranslate(x);
            RaiseOnChange();
        };
        tr.ManipulationDelta += (_, e) =>
        {
            e.Handled = true;
            if (isAuto)
                return;
            var x = e.Cumulative.Translation.X / ParentZoomFactor;
            var y = e.Cumulative.Translation.Y / ParentZoomFactor;
            TTranslate(y);
            RTranslate(x);
            RaiseOnChange();
        };
        bl.ManipulationDelta += (_, e) =>
        {
            e.Handled = true;
            if (isAuto)
                return;
            var x = e.Cumulative.Translation.X / ParentZoomFactor;
            var y = e.Cumulative.Translation.Y / ParentZoomFactor;
            BTranslate(y);
            LTranslate(x);
            RaiseOnChange();
        };
        br.ManipulationDelta += (_, e) =>
        {
            e.Handled = true;
            if (isAuto)
                return;
            var x = e.Cumulative.Translation.X / ParentZoomFactor;
            var y = e.Cumulative.Translation.Y / ParentZoomFactor;
            BTranslate(y);
            RTranslate(x);
            RaiseOnChange();
        };
        void LTranslate(double x)
        {
            if (sizeFromCenterMode)
                x *= 2;
            if (width - x >= 0)
            {
                if (resizerMode is ResizerMode.Inner && left + x < 0)
                {
                    // make addition 0
                    x = -left;
                }
                else if (resizerMode is ResizerMode.Outer && left + x > 0)
                {
                    // make addition 0
                    x = -left;
                }
                UserLeft = left + x;
                UserWidth = width - x;
            }
            else
            {
                if (resizerMode is ResizerMode.Outer && left + x < InitialSize.Width)
                {
                    // make addition 0
                    x = InitialSize.Width - left;
                }
                else if (resizerMode is ResizerMode.Inner && left + x > InitialSize.Width)
                {
                    // make addition 0
                    x = InitialSize.Width - left;
                }
                UserLeft = left + width;
                UserWidth = x - width;
            }
        }
        void RTranslate(double x)
        {
            if (sizeFromCenterMode)
                x *= 2;
            if (width + x >= 0)
            {
                if (resizerMode is ResizerMode.Outer && left + (width + x) < InitialSize.Width)
                {
                    // make addition 0
                    x = InitialSize.Width - left - width;
                }
                else if (resizerMode is ResizerMode.Inner && left + (width + x) > InitialSize.Width)
                {
                    // make addition 0
                    x = InitialSize.Width - left - width;
                }
                UserLeft = left;
                UserWidth = width + x;
            }
            else
            {
                if (resizerMode is ResizerMode.Inner && left + (width + x) < 0)
                {
                    // make addition 0
                    x = -(left + width);
                }
                else if (resizerMode is ResizerMode.Outer && left + (width + x) > 0)
                {
                    // make addition 0
                    x = -(left + width);
                }
                UserLeft = left + (width + x);
                UserWidth = -(width + x);
            }
        }
        void TTranslate(double y)
        {
            if (sizeFromCenterMode)
                y *= 2;
            
            if (height - y >= 0)
            {
                if (resizerMode is ResizerMode.Inner && top + y < 0)
                {
                    // make addition 0
                    y = -top;
                }
                else if (resizerMode is ResizerMode.Outer && top + y > 0)
                {
                    // make addition 0
                    y = -top;
                }
                UserTop = top + y;
                UserHeight = height - y;
            }
            else
            {
                if (resizerMode is ResizerMode.Outer && top + y < InitialSize.Height)
                {
                    // make addition 0
                    y = InitialSize.Height - top;
                }
                else if (resizerMode is ResizerMode.Inner && top + y > InitialSize.Height)
                {
                    // make addition 0
                    y = InitialSize.Height - top;
                }
                UserTop = top + height;
                UserHeight = y - height;
            }
        }
        void BTranslate(double y)
        {
            if (sizeFromCenterMode)
                y *= 2;
            if (height + y >= 0)
            {
                if (resizerMode is ResizerMode.Outer && top + (height + y) < InitialSize.Height)
                {
                    // make addition 0
                    y = InitialSize.Height - top - height;
                }
                else if (resizerMode is ResizerMode.Inner && top + (height + y) > InitialSize.Height)
                {
                    // make addition 0
                    y = InitialSize.Height - top - height;
                }
                UserTop = top;
                UserHeight = height + y;
            }
            else
            {
                if (resizerMode is ResizerMode.Inner && top + (height + y) < 0)
                {
                    // make addition 0
                    y = -(height + top);
                }
                else if (resizerMode is ResizerMode.Outer && top + (height + y) > 0)
                {
                    // make addition 0
                    y = -(height + top);
                }
                UserTop = top + (height + y);
                UserHeight = -(height + y);
            }
        }
    }
    void RaiseOnChange()
    {
        var initial = new Rect(0, 0, InitialSize.Width, InitialSize.Height);
        var current = new Rect(UserLeft, UserTop, CurrentWidth, CurrentHeight);
        OnChange(initial, current);
    }
    protected abstract void OnChange(Rect inputRectangle, Rect outputRectangle);

    protected bool AreCornersVisible
    {
        set
        {
            tl.IsVisible = tr.IsVisible = bl.IsVisible = br.IsVisible = value;
        }
    }

    protected bool AreSidesInteractable
    {
        set
        {
            _l.IsHitTestVisible = _r.IsHitTestVisible = _t.IsHitTestVisible = _b.IsHitTestVisible = value;
        }
    }

    protected bool AreMiddleHandlesVisible
    {
        set
        {
            l.IsHitTestVisible = r.IsHitTestVisible = t.IsHitTestVisible = b.IsHitTestVisible = value;
        }
    }
}
enum ResizerMode
{
    Inner,
    Outer,
    All
}