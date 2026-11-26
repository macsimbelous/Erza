using AnimatedImage;
using AnimatedImage.Avalonia;
using AnimatedImage.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using ErzaLib2;
using System;
using System.Collections.Generic;
using System.IO;

namespace Risa
{
    public partial class FullScreenWindow : Window
    {
        private int Index = -1;
        private List<ImageItem> AllImages;
        public FullScreenWindow()
        {
            InitializeComponent();
            // Подписываемся на событие на этапе туннелирования (PreviewKeyDown)
            this.AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        }
        private void OnOpened(object? sender, EventArgs e)
        {
            AllImages = ((MainViewModel)DataContext)._allImages;
            for (int i = 0; i < AllImages.Count; i++)
            {
                if (AllImages[i].IsSelected)
                {
                    Index = i; break;
                }
            }
            if (Index < 0) return;
            LoadImage();
        }
        public void LoadGifFromPath(string filePath)
        {
            if (!File.Exists(filePath)) return;

            // Преобразуем локальный путь в корректный file:// URI
            var fileUri = new Uri(Path.GetFullPath(filePath));

            // Назначаем источник анимации
            var stream = File.OpenRead(filePath);
            var source = new AnimatedImageSourceStream(stream);
            ImageBehavior.SetAnimatedSource(CurrentImage, source);

            // Сбрасываем масштаб под размер нового изображения
            ZoomBorder.ResetMatrix();
        }
        private void LoadImage()
        {
            if (AllImages.Count == 0) return;
            ImageItem item = AllImages[Index];
            if (Path.GetExtension(item.ImageInfo.FilePath).ToLower() == ".gif")
            {
                LoadGifFromPath(item.ImageInfo.FilePath);
            }
            else
            {
                var stream = File.OpenRead(item.ImageInfo.FilePath);
                CurrentImage.Source = new Bitmap(stream);
                ZoomBorder.ResetMatrix();
            }
        }
        private void OnWindowKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    // Действие при нажатии Вверх
                    Index--;
                    if (Index < 0)
                    {
                        if (AllImages.Count > 0)
                        {
                            Index = AllImages.Count - 1;
                        }
                        else
                        {
                            Index = 0;
                        }
                    }
                    LoadImage();
                    e.Handled = true; // Предотвращаем дальнейшую обработку (например, смену фокуса)
                    break;
                case Key.Down:
                    // Действие при нажатии Вниз
                    Index++;
                    if (Index >= AllImages.Count)
                    {
                        Index = 0;
                    }
                    LoadImage();
                    e.Handled = true;
                    break;
                case Key.Left:
                    // Действие при нажатии Влево
                    Index--;
                    if (Index < 0)
                    {
                        if (AllImages.Count > 0)
                        {
                            Index = AllImages.Count - 1;
                        }
                        else
                        {
                            Index = 0;
                        }
                    }
                    LoadImage();
                    e.Handled = true;
                    break;
                case Key.Right:
                    // Действие при нажатии Вправо
                    Index++;
                    if (Index >= AllImages.Count)
                    {
                        Index = 0;
                    }
                    LoadImage();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    // Действие при нажатии Esc
                    this.Close();
                    e.Handled = true;
                    break;
            }
        }
    }
}