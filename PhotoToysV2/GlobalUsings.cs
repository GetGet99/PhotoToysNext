global using Windows.UI.Core;
global using Windows.Foundation;
global using Windows.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls;
global using Windows.UI.Xaml;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using PhotoToysV2;
global using PhotoToysV2.Service;
global using PhotoToysV2.Effects;
global using PhotoToysV2.Controls;
global using PhotoToysV2.ImageDisplays;
global using ColorPicker = Microsoft.UI.Xaml.Controls.ColorPicker;
global using Get.UI.Data;
global using static Get.UI.Data.QuickCreate;
global using Windows.UI.Xaml.Media;
global using QuickMarkup.SourceGen;
global using QuickMarkup.Infra;
global using Windows.UI.Xaml.Markup;
global using Windows.UI;
global using Windows.Graphics.Effects;
global using Microsoft.Graphics.Canvas.Effects;
global using Microsoft.Graphics.Canvas;
global using static Properties;
global using static DrawHelper;
global using static QuickMarkup.Infra.QuickRefs;
global using FrameworkElementExtensions = CommunityToolkit.WinUI.FrameworkElementExtensions;

static class Properties
{
    public static DependencyProperty WinUIValueProperty => Windows.UI.Xaml.Controls.Primitives.RangeBase.ValueProperty;
}

static class DrawHelper
{
    static CanvasDevice device = CanvasDevice.GetSharedDevice();
    public static CanvasDrawingSession NewDrawing(Size size, out CanvasRenderTarget renderTarget)
    {
         try
        {
            renderTarget = new(device, size._width, size._height, 96);
            return renderTarget.CreateDrawingSession();
        } catch
        {
            renderTarget = new(device, 1, 1, 96);
            return renderTarget.CreateDrawingSession();
        }
    }
}