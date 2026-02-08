
namespace PhotoToysV2.Controls;

[QuickMarkup("""
    Rect Value = default;
    <root
        CurrentRect=`Value`
        CurrentRect=>`Value`
    />
    """)]
partial class CropResizer : ResizerUI
{
    public CropResizer() : base(ResizerMode.Inner, internalLayoutChanges: true)
    {
        Init();
        AreCornersVisible = true;
        AreMiddleHandlesVisible = true;
        AreSidesInteractable = true;
    }

    protected override void OnChange(Rect inputRectangle, Rect outputRectangle)
    {
        
    }

    public void Reset()
    {
        Value = default;
        ResetAll();
    }
}
