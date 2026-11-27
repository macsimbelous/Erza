using AnimatedImage.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;

namespace Risa
{
    public partial class SlideShowWindow : Window
    {
        public List<ImageItem> AllImages = new();
        public long Delay = 5;
        private readonly DispatcherTimer _timer;
        private int Index = 0;
        public SlideShowWindow()
        {
            InitializeComponent();
            // Подписываемся на событие на этапе туннелирования (PreviewKeyDown)
            this.AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);



            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Delay) };
            _timer.Tick += (s, e) =>
            {
                Index = (Index + 1) % AllImages.Count;
                LoadImage2();
            };
            _timer.Start();
        }
        public void LoadGifFromPath(string filePath)
        {
            if (!File.Exists(filePath)) return;

            // Преобразуем локальный путь в корректный file:// URI
            var fileUri = new Uri(Path.GetFullPath(filePath));

            // Назначаем источник анимации
            var stream = File.OpenRead(filePath);
            var source = new AnimatedImageSourceStream(stream);
            ImageBehavior.SetAnimatedSource(SlideControl, source);
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
                SlideControl.Content = new Bitmap(stream);
            }
        }
        private void LoadImage2()
        {
            if (AllImages.Count == 0) return;
            ImageItem item = AllImages[Index];
            SlideControl.Content = item.ImageInfo.FilePath;
        }
        private void OnOpened(object? sender, EventArgs e)
        {
            if (AllImages.Count > 0)
                LoadImage2();
        }
        // Останавливаем таймер при закрытии окна во избежание утечек памяти
        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
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
                    LoadImage2();
                    ResetTimer();
                    e.Handled = true; // Предотвращаем дальнейшую обработку (например, смену фокуса)
                    break;
                case Key.Down:
                    // Действие при нажатии Вниз
                    Index++;
                    if (Index >= AllImages.Count)
                    {
                        Index = 0;
                    }
                    LoadImage2();
                    ResetTimer();
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
                    LoadImage2();
                    ResetTimer();
                    e.Handled = true;
                    break;
                case Key.Right:
                    // Действие при нажатии Вправо
                    Index++;
                    if (Index >= AllImages.Count)
                    {
                        Index = 0;
                    }
                    LoadImage2();
                    ResetTimer();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    // Действие при нажатии Esc
                    this.Close();
                    e.Handled = true;
                    break;
            }
        }
        // Метод для сброса отсчета
        private void ResetTimer()
        {
            _timer.Stop();
            // Если нужно одновременно изменить длительность интервала, раскомментируйте строку ниже:
            // _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Start();
        }
    }
}