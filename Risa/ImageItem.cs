using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ErzaLib2;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Risa;

public partial class ImageItem : ObservableObject
{
    public string? FilePath => ImageInfo.FilePath;
    public string? Title { get; }
    public ImageInfo ImageInfo { get; }
    private string ThumbnailPath;
    // Ограничиваем чтение с диска до 6 параллельных потоков
    private static readonly SemaphoreSlim _diskSemaphore = new(6, 6);
    private CancellationTokenSource? _loadCts;

    [ObservableProperty]
    private Bitmap? _thumbnail;

    [ObservableProperty]
    private bool _isSelected;

    private bool _isLoading;

    public ImageItem(ImageInfo imageinfo)
    {
        ImageInfo = imageinfo;
        Title = Path.GetFileName(imageinfo.FilePath);
        ThumbnailPath = MainWindow.PreviewPath + "\\" + imageinfo.Hash[0] + "\\" + imageinfo.Hash[1] + "\\" + imageinfo.Hash + ".jpg";
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
                //return Bitmap.DecodeToWidth(stream, PreviewWidth);
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
    public async void LoadThumbnailAsync()
    {
        if (Thumbnail != null || _loadCts != null) return;

        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;

        try
        {
            await _diskSemaphore.WaitAsync(token);
            try
            {
                var bitmap = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var stream = File.OpenRead(ThumbnailPath);
                    //return Bitmap.DecodeToWidth(stream, 160, BitmapInterpolationMode.LowQuality);
                    return new Bitmap(stream);
                }, token);

                if (!token.IsCancellationRequested)
                {
                    await Dispatcher.UIThread.InvokeAsync(() => Thumbnail = bitmap);
                }
                else
                {
                    bitmap.Dispose();
                }
            }
            finally
            {
                _diskSemaphore.Release();
            }
        }
        catch
        {
            // Отмена при быстрой прокрутке или ошибка доступа к файлу
        }
        finally
        {
            _loadCts?.Dispose();
            _loadCts = null;
        }
    }

    public void UnloadThumbnail()
    {
        _loadCts?.Cancel();

        if (Thumbnail != null)
        {
            var old = Thumbnail;
            Thumbnail = null;
            old.Dispose();
        }
    }
}
