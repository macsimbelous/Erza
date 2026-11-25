using AnimatedImage;
using AnimatedImage.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ErzaLib2;
using HarfBuzzSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace Risa
{
    public partial class ViewImageWindow : Window
    {
        public ImageInfo CurrentImageInfo { get; set; }
        private int Index = -1;
        private List<ImageItem> AllImages;
        private Bitmap FavoritedImage;
        private Bitmap NotFavoritedImage;
        public ObservableCollection<RisaTagInfo> ImageTags;
        public List<string> SelectedTags = new List<string>();
        public ViewImageWindow()
        {
            InitializeComponent();

            // Подписываемся на событие на этапе туннелирования (PreviewKeyDown)
            this.AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);

            var uri = new Uri("avares://Risa/Assets/favorited.png");
            using var stream = AssetLoader.Open(uri);
            FavoritedImage = new Bitmap(stream);
            uri = new Uri("avares://Risa/Assets/not_favorited.png");
            using var stream2 = AssetLoader.Open(uri);
            NotFavoritedImage = new Bitmap(stream2);
        }
        // Метод для загрузки GIF по известному пути на диске
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
            if (item.ImageInfo.Favorited)
            {
                FavoritedButton.Source = FavoritedImage;
            }
            else
            {
                FavoritedButton.Source = NotFavoritedImage;
            }
            ImageTags = GetTags(item.ImageInfo.ImageID);
            TagsListBox.ItemsSource = ImageTags;
            CountTagsTextBlock.Text = ImageTags.Count.ToString();
            ImageFormatTextBlock.Text = Path.GetExtension(item.ImageInfo.FilePath).ToUpper().Remove(0, 1);
            ImageResolutionTextBlock.Text = CurrentImage.Source.Size.ToString();
            ImageFileSizeTextBlock.Text = new FileInfo(item.ImageInfo.FilePath).Length.ToString();
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
            }
        }
        private void FavoritedButtonClick(object? sender, RoutedEventArgs e)
        {
            ImageInfo img = AllImages[Index].ImageInfo;
            if (img.Favorited)
            {
                ErzaDB.SetImageFavorit(img.ImageID, false, MainWindow.Erza);
                FavoritedButton.Source = NotFavoritedImage;
                img.Favorited = false;
            }
            else
            {
                ErzaDB.SetImageFavorit(img.ImageID, true, MainWindow.Erza);
                FavoritedButton.Source = FavoritedImage;
                img.Favorited = true;
            }
        }
        private void SearchButtonClick(object? sender, RoutedEventArgs e)
        {
            this.SelectedTags = new List<string>();
            foreach (var item in TagsListBox.SelectedItems)
            {
                RisaTagInfo t = (RisaTagInfo)item;
                this.SelectedTags.Add(t.Tag);
            }
            this.Close();
        }
        private async void AddTagsButtonClick(object? sender, RoutedEventArgs e)
        {
            var dialog = new AddTagsWindow();

            // Ждём закрытия и получаем результат (если закрыли крестиком — вернётся null)
            string? result = await dialog.ShowDialog<string>(this);

            if (result != null)
            {
                ImageInfo img = AllImages[Index].ImageInfo;
                string[] tags = result.Trim().Split(' ');
                img.AddTags(tags);
                ErzaDB.LoadImageToErza(img, MainWindow.Erza);
                ImageTags = GetTags(img.ImageID);
                TagsListBox.ItemsSource = ImageTags;
                CountTagsTextBlock.Text = ImageTags.Count.ToString();
            }
        }
        private async void RemoveTagsButtonClick(object? sender, RoutedEventArgs e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.YesNo,
                ContentTitle = "Подтверждение",
                ContentMessage = "Выбрано " + TagsListBox.SelectedItems.Count + "тегов удалить?",
                Icon = MsBox.Avalonia.Enums.Icon.Question,
                // Открыть строго по центру родительского окна
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });
            // Обязательно передаем 'this' (ссылку на родительский Window)
            var result = await box.ShowWindowDialogAsync(this);
            if (result == ButtonResult.Yes)
            {
                List<RisaTagInfo> tags = new List<RisaTagInfo>();
                foreach (RisaTagInfo item in TagsListBox.SelectedItems)
                {
                    tags.Add(item);
                    ErzaDB.DeleteTagFromImage(item.TagID, AllImages[Index].ImageInfo.ImageID, MainWindow.Erza);
                }
                foreach(RisaTagInfo item in tags)
                {
                    ImageTags.Remove(item);
                }
            }
        }
        private ObservableCollection<RisaTagInfo> GetTags(long ImageID)
        {
            List<TagInfo> temp = ErzaDB.GetTagsByImageID(ImageID, MainWindow.Erza);
            //var artist = temp.Where(t => t.Type == TagType.Artist);
            //var copyright = temp.Where(t => t.Type == TagType.Copyright);
            //var character = temp.Where(t => t.Type == TagType.Character);
            //var studio = temp.Where(t => t.Type == TagType.Studio);
            //var circle = temp.Where(t => t.Type == TagType.Circle);
            //var medium = temp.Where(t => t.Type == TagType.Medium);
            //var meta = temp.Where(t => t.Type == TagType.Meta);
            //var faults = temp.Where(t => t.Type == TagType.Faults);
            //var general = temp.Where(t => t.Type == TagType.General);
            var lists = new[]
            {
                temp.Where(t => t.Type == TagType.Artist),
                temp.Where(t => t.Type == TagType.Copyright),
                temp.Where(t => t.Type == TagType.Character),
                temp.Where(t => t.Type == TagType.Studio),
                temp.Where(t => t.Type == TagType.Circle),
                temp.Where(t => t.Type == TagType.Medium),
                temp.Where(t => t.Type == TagType.Meta),
                temp.Where(t => t.Type == TagType.Faults),
                temp.Where(t => t.Type == TagType.General)
            };
            ObservableCollection<RisaTagInfo> image_tags = new ObservableCollection<RisaTagInfo>();
            foreach (var list in lists)
            {
                var tags = list.OrderBy(t => t.Tag);
                foreach (TagInfo tag in tags)
                {
                    RisaTagInfo rti = new RisaTagInfo()
                    {
                        TagID = tag.TagID,
                        Count = tag.Count,
                        Type = tag.Type,
                        Tag = tag.Tag,
                        Localization = tag.Localization,
                        Description = tag.Description
                    };
                    image_tags.Add(rti);
                }
            }
            return image_tags;
        }
    }
    public class RisaTagInfo
    {
        public long TagID { get; set; } = -1;
        public long Count { get; set; } = 0;
        public TagType Type { get; set; } = TagType.General;
        public string? Tag { get; set; }
        public string? Localization { get; set; }
        public string? Description { get; set; }
    }
}