using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ErzaLib2;
namespace Risa;

public partial class ImageItem : ObservableObject
{
    public string? FilePath => ImageInfo.FilePath;
    public string? Title { get; }
    public ImageInfo ImageInfo { get; }
    private string ThumbnailPath;

    [ObservableProperty]
    private Bitmap? _thumbnail;

    [ObservableProperty]
    private bool _isSelected;

    private bool _isLoading;

    public ImageItem(ImageInfo imageinfo)
    {
        ImageInfo = imageinfo;
        Title = Path.GetFileName(imageinfo.FilePath);
        ThumbnailPath = MainWindow.PreviewPath + "\\" + ImageInfo.Hash[0] + "\\" + ImageInfo.Hash[1] + "\\" + ImageInfo.Hash + ".jpg";
    }

    // Вызывается только тогда, когда строка попадает в видимую область экрана
    public async void EnsureLoaded()
    {
        if (Thumbnail != null || _isLoading || !File.Exists(ThumbnailPath)) return;
        _isLoading = true;

        try
        {
            Thumbnail = await Task.Run(() =>
            {
                //using var stream = File.OpenRead(FilePath);
                //return Bitmap.DecodeToWidth(stream, 200);
                using var stream = File.OpenRead(ThumbnailPath);
                return new Bitmap(stream);
            });
        }
        catch { /* Игнорируем битые файлы */ }
        finally
        {
            _isLoading = false;
        }
    }
}
