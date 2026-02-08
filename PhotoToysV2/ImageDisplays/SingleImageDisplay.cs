using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace PhotoToysV2.ImageDisplays;

[QuickMarkup("""
    using Microsoft.Graphics.Canvas.UI.Xaml;
    bool ShouldAskToOpen = false;
    IImageEffect? ImageEffect;
    private ICanvasImage? InputImage = null;
    bool HasInputImage => `InputImage is not null`;
    private ICanvasImage? PreviewImage => `ApplyEffect(ImageEffect, InputImage)`;
    private Size InputSize => /-() => {
        var image = InputImage;
        if (image is null)
        {
            return new Size(double.NaN, double.NaN);
        }
        var bounds = image.Bounds;
        return new Size(bounds.Width, bounds.Height);
    }-/;
    private Size OutputSize => /-() => {
        var image = PreviewImage;
        if (image is null)
        {
            return new Size(double.NaN, double.NaN);
        }
        var bounds = image.Bounds;
        return new Size(bounds.Width, bounds.Height);
    }-/;
    private ICanvasImage? DisplayImage => `ShouldShowOriginal ? (InputImage ?? PreviewImage) : PreviewImage`;
    private Size DisplaySize => /-ShouldShowOriginal
        ? (double.IsNaN(InputSize.Width) ? OutputSize : InputSize)
        : OutputSize-/;
    bool ShouldShowOriginal = false;
    <root>
        imgPicker = <ImagePicker MaxWidth=750 MaxHeight=500 Margin=16
            IsVisible=`ShouldAskToOpen && InputImage is null && ImageEffect is not ICreateImageEffect`
        />
        scrollViewer = <ScrollViewer
            IsVisible=`InputImage is not null || ImageEffect is ICreateImageEffect`
            HorizontalScrollBarVisibility=Auto HorizontalScrollMode=Auto
            ZoomMode=Enabled
        >
            <Grid
                Width=`DisplaySize.Width`
                Height=`DisplaySize.Height`
            >
                canvas = <CanvasControl
                    Width=`DisplaySize.Width`
                    Height=`DisplaySize.Height`
                    Draw+=`Draw`
                />
                resizerPlace = <Canvas
                    Width=`DisplaySize.Width`
                    Height=`DisplaySize.Height`
                />
            </Grid>
        </ScrollViewer>
    </root>
    """)]
partial class SingleImageDisplay : Grid
{
    private ICanvasImage? FinalImage => ApplyEffect(ImageEffect, InputImage, preview: false);
    // public getter
    public ICanvasImage? CurrentDisplay => InputImage;
    
    readonly CanvasDevice device = CanvasDevice.GetSharedDevice();
    ICanvasImage? ApplyEffect(IImageEffect? effect, ICanvasImage? img, bool preview = true)
    {
        return ReferenceTracker.NoCapture(() => effect switch
        {
            ISingleImageEffectPreview single =>
                img is null ? null :
                (preview ? single.GetPreview(img) : single.GetEffect(img)),
            ISingleImageEffect single =>
                img is null ? null :
                single.GetEffect(img),
            ICreateImageEffectPreview created =>
                preview ? created.GetPreview() : created.GetEffect(),
            ICreateImageEffect created => created.GetEffect(),
            null => img,
            _ => throw new NotImplementedException()
        });
    }
    void Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (DisplayImage is { } img)
        {
            args.DrawingSession.DrawImage(img, 0, 0);
        }
    }

    public SingleImageDisplay()
    {
        Name = nameof(SingleImageDisplay);
        IImageEffect? old = null;
        Init();
        ShouldShowOriginalProp.Watch(_ => canvas.Invalidate());
        ImageEffectProp.Watch(x =>
        {
            void Redraw()
            {
                PreviewImageComp.Invalidate();
                canvas.Invalidate();
            }
            old?.ParametersUpdated -= Redraw;
            x?.ParametersUpdated += Redraw;
            Redraw();
            old = x;
        });
        InputImageProp.Watch(x =>
        {
            if (x is not null)
            {
                UpdateImageZoom(x);
            }
        });
        Effect(() =>
        {
            var newImage = imgPicker.InputImage;
            if (newImage is null) return;
            // push current image (may be null)
            History.Current.DoAction(InputImage);
            InputImage = newImage;
            UpdateImageZoom(newImage);
        }, imgPicker.InputImageProp);
        this.FirstLoadedEv(() =>
        {
            var resizerService = ResizerService.Instance;
            resizerService.ResizerProp.Watch(x =>
            {
                if (resizerPlace.Children.Count is not 0)
                    resizerPlace.Children.RemoveAt(0);
                if (x is not null) {
                    resizerPlace.Children.Add(x);
                    UpdateSize(x, OutputSize);
                }
            });
            OutputSizeComp.Watch(x =>
            {
                if (resizerService.Resizer is { } resizer)
                    UpdateSize(resizer, x);
            });
            scrollViewer.ViewChanged += delegate
            {
                if (resizerService.Resizer is { } resizer)
                    UpdateSize(resizer, OutputSize);
            };
            void UpdateSize(ResizerUI resizer, Size inputSize)
            {
                resizer.InitialSize = inputSize;
                resizer.ParentZoomFactor = scrollViewer.ZoomFactor;
            }
        });
    }

    void UpdateImageZoom(ICanvasImage? input)
    {
        if (input is null)
        {
            scrollViewer.ZoomToFactor(1);
            return;
        }
        var bounds = input.Bounds;
        scrollViewer.ZoomToFactor(MathF.Min(
                (float)(Math.Min(750, ActualWidth - 32) / bounds.Width),
                (float)(Math.Min(500, ActualHeight - 32) / bounds.Height)
            ));
        scrollViewer.ScrollToHorizontalOffset(ActualWidth / 2);
        scrollViewer.ScrollToVerticalOffset(ActualHeight / 2);
    }
    public void Reset()
    {
        History.Current.DoAction(InputImage);
        InputImage = null;
        UpdateImageZoom(null);
    }
    public void Undo()
    {
        if (History.Current.CanUndo)
        {
            InputImage = History.Current.Undo(InputImage);
            canvas.Invalidate();
            UpdateImageZoom(InputImage);
        }
    }
    public void Redo()
    {
        if (History.Current.CanRedo)
        {
            InputImage = History.Current.Redo(InputImage);
            canvas.Invalidate();
            UpdateImageZoom(InputImage);
        }
    }
    public void Apply()
    {
        if (FinalImage is { } finalImg)
        {
            History.Current.DoAction(InputImage);
            InputImage = finalImg;
            UpdateImageZoom(finalImg);
            if (ImageEffect is IImageEffectApplyNotify applyNotify)
            {
                applyNotify.Applied();
            }
        }
    }

    public async void Save()
    {
#pragma warning disable IDE0028 // Simplify collection initialization
        FileSavePicker fileSavePicker = new()
        {
            DefaultFileExtension = ".png",
            FileTypeChoices =
            {
                ["PNG"] = new List<string>()
                {
                    ".png"
                },
                ["JPEG"] = new List<string>()
                {
                    ".jpg", ".jpeg"
                },
                ["TIFF"] = new List<string>()
                {
                    ".tiff"
                },
                ["GIF"] = new List<string>()
                {
                    ".gif"
                },
                ["BMP"] = new List<string>()
                {
                    ".bmp"
                }
            }
        };
#pragma warning restore IDE0028 // Simplify collection initialization
        var file = await fileSavePicker.PickSaveFileAsync();
        if (file is not null)
        {
            using var renderTarget = GetFinalRenderTarget() ?? throw new Exception("Render Target is null");
            using var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            await renderTarget.SaveAsync(
                stream,
                file.FileType switch
                {
                    ".bmp" => CanvasBitmapFileFormat.Bmp,
                    ".gif" => CanvasBitmapFileFormat.Gif,
                    ".jpg" or ".jpeg" => CanvasBitmapFileFormat.Jpeg,
                    ".png" => CanvasBitmapFileFormat.Png,
                    ".tiff" => CanvasBitmapFileFormat.Tiff,
                    _ => throw new Exception()
                }
            );
        }
    }

    public async void Copy()
    {
        using var renderTarget = GetFinalRenderTarget() ?? throw new Exception("Render Target is null");
        // Save CanvasRenderTarget to an in-memory stream
        InMemoryRandomAccessStream stream = new();
        await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);

        // Prepare clipboard data
        DataPackage dataPackage = new()
        {
            RequestedOperation = DataPackageOperation.Copy
        };
        dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
        dataPackage.SetData("PNG", stream);

        // Set clipboard
        Clipboard.SetContent(dataPackage);
    }
    CanvasRenderTarget? GetFinalRenderTarget()
    {
        if (FinalImage is { } img)
        {
            var bounds = img.Bounds;
            var renderTarget = new CanvasRenderTarget(
                device,
                bounds._width, bounds._height,
                imgPicker.InputImage?.Dpi ?? 96
            );
            using var ds = renderTarget.CreateDrawingSession();
            ds.DrawImage(img);
            return renderTarget;
        }
        return null;
    }
}
