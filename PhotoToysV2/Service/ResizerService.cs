namespace PhotoToysV2.Service;

[QuickMarkup("""
    ResizerUI? Resizer = null;
    """)]
partial class ResizerService
{
    public static ResizerService Instance { get; } = new ResizerService();
}
