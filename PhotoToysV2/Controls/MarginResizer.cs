
namespace PhotoToysV2.Controls;

partial class MarginResizer : ResizerUI
{
    public MarginResizer() : base(ResizerMode.Outer, internalLayoutChanges: false)
    {
        AreCornersVisible = false;
        AreMiddleHandlesVisible = true;
        AreSidesInteractable = true;
    }
    protected override void OnChange(Rect inputRectangle, Rect outputRectangle) { }
    public Thickness Value { 
        get
        {
            var inputRectangle = new Rect(0, 0, InitialSize.Width, InitialSize.Height);
            var outputRectangle = CurrentRect;
            return new Thickness(
                left: inputRectangle.Left - outputRectangle.Left,
                top: outputRectangle.Top - inputRectangle.Top,
                right: outputRectangle.Right - inputRectangle.Right,
                bottom: outputRectangle.Bottom - inputRectangle.Bottom
            );
        } set
        {
            // do nothing yet
        }
    }
}
