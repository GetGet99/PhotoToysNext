
namespace PhotoToysV2.Controls;

[QuickMarkup("""
    Rect Value = default;
    <root
        CurrentRect=/-Value-/
        CurrentRect=>/-Value-/
    />
    """)]
partial class CreateResizer : ResizerUI
{
    public CreateResizer() : base(ResizerMode.All, internalLayoutChanges: false, sizeFromCenterMode: true)
    {
        Init();
        AreCornersVisible = true;
        AreMiddleHandlesVisible = true;
        AreSidesInteractable = true;
    }

    protected override void OnChange(Rect inputRectangle, Rect outputRectangle)
    {
        //Value = outputRectangle;
    }

    public void Reset()
    {
        Value = default;
        ResetAll();
    }
}
