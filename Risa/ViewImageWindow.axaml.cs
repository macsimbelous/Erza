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
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;

namespace Risa
{
    public partial class ViewImageWindow : Avalonia.Controls.Window
    {
        const int ImageSize = 448; // MOAT Tagger uses 448x448 input size
        const float Threshold = 0.4772f; // Standard F1 threshold recommendation
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
                case Key.Escape:
                    // Действие при нажатии Esc
                    this.Close();
                    e.Handled = true;
                    break;
                case Key.Delete:
                    // Действие при нажатии Del
                    MainViewModel vm = (MainViewModel)DataContext;
                    vm.RemoveSelected(new List<ImageItem>((IEnumerable<ImageItem>)AllImages[Index]));
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
        private async void GenAITagsButtonClick(object? sender, RoutedEventArgs e)
        {
            string modelPath = @"C:\utils\data\models\wd-eva02-large-tagger-v3\model.onnx";
            string csvPath = @"C:\utils\data\models\wd-eva02-large-tagger-v3\selected_tags.csv";
            string imagePath = AllImages[Index].ImageInfo.FilePath;
            if (!File.Exists(modelPath) || !File.Exists(csvPath) || !File.Exists(imagePath))
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Ошибка!",
                    ContentMessage = "Please ensure model.onnx, selected_tags.csv, and input.jpg exist.",
                    Icon = MsBox.Avalonia.Enums.Icon.Error,
                    // Открыть строго по центру родительского окна
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                // Обязательно передаем 'this' (ссылку на родительский Window)
                await box.ShowWindowDialogAsync(this);
                return;
            }
            // 1. Load the tag names mapping from the CSV file
            var tags = LoadTags(csvPath);
            // 2. Preprocess the image
            float[] inputData = PreprocessImage(imagePath);
            // 3. Prepare the ONNX Tensor Input
            var inputMeta = new List<NamedOnnxValue>
            {
                //NamedOnnxValue.CreateFromTensor("input_1:0", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
                NamedOnnxValue.CreateFromTensor("input", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
            };
            // 4. Run Inference via ONNX Runtime
            using var session = new InferenceSession(modelPath);
            //int gpuDeviceId = 0; // The GPU device ID to execute on
            //using var gpuSessionOptoins = SessionOptions.MakeSessionOptionWithCudaProvider(gpuDeviceId);
            //using var session = new InferenceSession(modelPath, gpuSessionOptoins);

            /*foreach (var input in session.InputMetadata)
            {
                Console.WriteLine($"Expected Input Name: {input.Key}");
            }*/
            using var results = session.Run(inputMeta);

            // Get output data (typically named "output_1" or the first item)
            var outputTensor = results.First().AsTensor<float>();
            float[] probabilities = outputTensor.ToArray();

            // 5. Match results with tags and filter by threshold
            //var predictedTags = new List<(string Tag, float Confidence, int Category)>();
            List<string> temp = new List<string>();
            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] >= Threshold && i < tags.Count)
                {
                    //predictedTags.Add((tags[i].Name, probabilities[i], tags[i].Category));
                    temp.Add(tags[i].Name);
                }
            }
            AllImages[Index].ImageInfo.AddTags(temp.ToList());
            AllImages[Index].ImageInfo.Tags = AllImages[Index].ImageInfo.Tags.Distinct().ToList();
            ErzaDB.LoadImageToErza(AllImages[Index].ImageInfo, MainWindow.Erza);
            List<TagInfo> temp2 = ErzaDB.GetTagsByImageID(AllImages[Index].ImageInfo.ImageID, MainWindow.Erza);
            ImageTags = GetTags(AllImages[Index].ImageInfo.ImageID);
            TagsListBox.ItemsSource = ImageTags;
            CountTagsTextBlock.Text = ImageTags.Count.ToString();
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
        private float[] PreprocessImage(string path)
        {
            // Load using OpenCvSharp
            using Mat src = Cv2.ImRead(path, ImreadModes.Color);
            if (src.Empty()) throw new Exception("Could not load image.");

            // Resize to 448x448
            using Mat resized = new Mat();
            Cv2.Resize(src, resized, new OpenCvSharp.Size(ImageSize, ImageSize), 0, 0, InterpolationFlags.Cubic);

            // Convert BGR (OpenCV Default) to RGB
            using Mat rgb = new Mat();
            Cv2.CvtColor(resized, rgb, ColorConversionCodes.BGR2RGB);

            // Convert byte array to float array normalized to 0.0f - 255.0f (WD models don't use 0-1 scaling)
            // Format: NHWC -> [1, 448, 448, 3]
            float[] floatBuffer = new float[ImageSize * ImageSize * 3];
            int index = 0;

            for (int y = 0; y < ImageSize; y++)
            {
                for (int x = 0; x < ImageSize; x++)
                {
                    Vec3b pixel = rgb.At<Vec3b>(y, x);
                    floatBuffer[index++] = pixel.Item0; // R
                    floatBuffer[index++] = pixel.Item1; // G
                    floatBuffer[index++] = pixel.Item2; // B
                }
            }

            return floatBuffer;
        }

        private List<(string Name, int Category)> LoadTags(string csvPath)
        {
            var tagsList = new List<(string Name, int Category)>();
            var lines = File.ReadAllLines(csvPath);

            // Skip the header (tag_id, name, category, count)
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Simple splitting logic. Be aware some tags may contain commas, but Danbooru tags use underscores.
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    string name = parts[1].Trim('"');
                    int category = int.Parse(parts[2]);
                    tagsList.Add((name, category));
                }
            }
            return tagsList;
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