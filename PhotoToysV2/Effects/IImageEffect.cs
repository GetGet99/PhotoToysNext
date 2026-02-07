namespace PhotoToysV2.Effects;

interface IImageEffect
{
    string DisplayName { get; }
    IEnumerable<string> Keywords { get; }
    event Action ParametersUpdated;
}
interface ISingleImageEffect : IImageEffect
{
    ICanvasImage GetExample(ICanvasImage input);
    ICanvasImage GetEffect(ICanvasImage input);
}
interface ISingleImageEffectPreview : ISingleImageEffect
{
    ICanvasImage GetPreview(ICanvasImage input);
}
interface ICreateImageEffect : IImageEffect
{
    ICanvasImage GetExample();
    ICanvasImage GetEffect();
}
interface ICreateImageEffectPreview : ICreateImageEffect
{
    ICanvasImage GetPreview();
}
interface IImageEffectSelectNotify : IImageEffect
{
    void Selected();
    void Deselected();
}
interface IImageEffectApplyNotify : IImageEffect
{
    void Applied();
}