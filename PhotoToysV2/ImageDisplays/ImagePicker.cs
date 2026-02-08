using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace PhotoToysV2;

[QuickMarkup("""
    CanvasBitmap? InputImage;
    <setup>
    var subtitleStyle = (Style)Application.Current.Resources["SubtitleTextBlockStyle"];
    </setup>
    <root AllowDrop DragOver+=`OnDragOver` Drop+=`OnDrop`>
        <VStack Spacing=16 Center>
            <TextBlock Text="Drop Image here" Style=`subtitleStyle` CenterH />
            <TextBlock Text="or" CenterH />
            <Button Content="Select an image from folder" CenterH Click+=`(_, _) => PickImageFromFolder()` />
            <Button Content="Paste Image" CenterH Click+=`(_, _) => GetFromClipboard()` />
        </VStack>
    </root>
    """)]
partial class ImagePicker : Card
{
    readonly CanvasDevice device = CanvasDevice.GetSharedDevice();
    async void PickImageFromFolder()
    {
        try
        {
            FileOpenPicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".bmp",
                    ".tiff",
                    ".png",
                    ".gif"
                }
            };
            var file = await picker.PickSingleFileAsync();
            if (file is not null)
            {
                using var stream = await file.OpenAsync(FileAccessMode.Read);
                InputImage = await CanvasBitmap.LoadAsync(device, stream);
            }
        }
        catch
        {

        }
    }
    async void GetFromClipboard()
    {
        LoadFromDataPcakageView(Clipboard.GetContent());
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        try
        {
            if (
                e.DataView.AvailableFormats.Contains(StandardDataFormats.Bitmap) ||
                e.DataView.AvailableFormats.Contains("PNG") ||
                e.DataView.AvailableFormats.Contains(StandardDataFormats.StorageItems)
                )
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }
        catch
        {

        }
    }

    async void LoadFromDataPcakageView(DataPackageView DataView)
    {
        try
        {
            if (DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await DataView.GetStorageItemsAsync();
                var file = items.OfType<StorageFile>().First();

                using var stream = await file.OpenAsync(FileAccessMode.Read);
                InputImage = await CanvasBitmap.LoadAsync(device, stream);
            }
            if (DataView.Contains("PNG"))
            {
                var item = await DataView.GetDataAsync("PNG");
                if (item is IRandomAccessStream stream)
                {
                    InputImage = await CanvasBitmap.LoadAsync(device, stream);
                }
            }
            if (DataView.Contains(StandardDataFormats.Bitmap))
            {
                var bmp = await DataView.GetBitmapAsync();
                using var stream = await bmp.OpenReadAsync();
                InputImage = await CanvasBitmap.LoadAsync(device, stream);
            }
        } catch
        {

        }
    }
    private async void OnDrop(object sender, DragEventArgs e)
        => LoadFromDataPcakageView(e.DataView);
}
