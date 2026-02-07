using Windows.UI.Xaml.Shapes;

namespace PhotoToysV2.Controls;

partial class SliderFromCenter : Slider
{
    protected override void OnApplyTemplate()
    {
        var oldForegroundPart = (Rectangle)GetTemplateChild("HorizontalDecreaseRect");
        oldForegroundPart.Opacity = 0;
        var grid = (Grid)VisualTreeHelper.GetParent(oldForegroundPart);
        var newForegroundPart = new Rectangle
        {
            RadiusX = oldForegroundPart.RadiusX,
            RadiusY = oldForegroundPart.RadiusY,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        newForegroundPart.Fill = oldForegroundPart.Fill;
        RegisterPropertyChangedCallback(ForegroundProperty, delegate
        {
            newForegroundPart.Fill = Foreground;
        });
        Grid.SetColumnSpan(newForegroundPart, 3);
        Grid.SetRow(newForegroundPart, 1);
        grid.Children.Insert(grid.Children.IndexOf(oldForegroundPart), newForegroundPart);
        oldForegroundPart.SizeChanged += delegate
        {
            newForegroundPart.Height = oldForegroundPart.Height;
            var pos = oldForegroundPart.Width;
            if (double.IsNaN(pos))
                pos = 0;
            var halfway = grid.ActualWidth / 2;
            if (pos > halfway)
            {
                newForegroundPart.Margin = new(halfway, 0, 0, 0);
                newForegroundPart.Width = pos - halfway;
            } else
            {
                newForegroundPart.Margin = new(pos, 0, 0, 0);
                newForegroundPart.Width = halfway - pos;
            }
        };
        base.OnApplyTemplate();
    }
}