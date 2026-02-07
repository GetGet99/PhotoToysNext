namespace PhotoToysV2;

[QuickMarkup("""
    // TODO: private set
    bool CanUndo = false;
    bool CanRedo = false;
    """)]
public partial class History
{
    public static History Current => field ??= new();
    Stack<ICanvasImage?> Undoables { get; } = [];
    Stack<ICanvasImage?> Redoables { get; } = [];

    public void DoAction(ICanvasImage? current)
    {
        Undoables.Push(current);
        CanUndo = true;
        CanRedo = false;
        Redoables.Clear();
    }
    public ICanvasImage? Undo(ICanvasImage? current)
    {
        var item = Undoables.Pop();
        CanUndo = Undoables.Count > 0;
        Redoables.Push(current);
        CanRedo = true;
        return item;
    }
    public ICanvasImage? Redo(ICanvasImage? current)
    {
        var item = Redoables.Pop();
        CanRedo = Redoables.Count > 0;
        Undoables.Push(current);
        CanUndo = true;
        return item;
    }
}