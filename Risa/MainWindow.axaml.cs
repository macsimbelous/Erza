using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using ErzaLib2;
using HarfBuzzSharp;
using ImageMagick;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tmds.DBus.Protocol;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Risa;

public partial class MainWindow : Window
{
    public static SQLiteConnection Erza;
    public static string PreviewPath = "D:\\previews";
    private string WallPath = "D:\\Wallpapers\\";
    private const int PreviewWidth = 300;
    private const int PreviewHeight = 225;
    public ObservableCollection<string> Tags { get; } = new();
    public MainWindow()
    {
        InitializeComponent();
        Erza = new SQLiteConnection(@"data source=C:\utils\data\erza.sqlite");
        Erza.Open();
        using (SQLiteCommand command = new SQLiteCommand(Erza))
        {
            command.CommandText = "SELECT tag, count FROM tags ORDER BY tag ASC";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string tag = reader.GetString(0);
                    if (String.IsNullOrEmpty(tag))
                    {
                        continue;
                    }
                    if (String.IsNullOrWhiteSpace(tag))
                    {
                        continue;
                    }
                    if (tag.IndexOf('\n') >= 0)
                    {
                        continue;
                    }
                    if (tag.IndexOf(' ') >= 0)
                    {
                        continue;
                    }
                    if (tag.IndexOf('\t') >= 0)
                    {
                        continue;
                    }
                    if (tag.IndexOf('\r') >= 0)
                    {
                        continue;
                    }
                    if (tag.IndexOf((char)12288) >= 0)
                    {
                        continue;
                    }
                    Tags.Add(tag);
                }
            }
        }
        autobox.ItemsSource = Tags;
        // 1. Фильтруем элементы только по последнему введенному слову
        autobox.ItemFilter = (searchText, item) =>
        {
            if (string.IsNullOrWhiteSpace(searchText) || item is not string word)
                return false;

            // Берем часть текста после последнего пробела
            var lastWord = GetLastWord(searchText);
            if (string.IsNullOrEmpty(lastWord))
                return false;

            return word.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase);
        };

        // 2. При выборе элемента из списка формируем итоговую строку
        autobox.TextSelector = (searchText, selectedItem) =>
        {
            if (string.IsNullOrEmpty(searchText))
                return selectedItem;

            int lastSpaceIndex = searchText.LastIndexOf(' ');

            // Если пробелов не было, просто возвращаем выбранное слово + пробел
            if (lastSpaceIndex == -1)
                return selectedItem + " ";

            // Оставляем текст до последнего пробела и приклеиваем выбранное слово
            string prefix = searchText.Substring(0, lastSpaceIndex + 1);
            return prefix + selectedItem + " ";
        };
    }
    // Вызывается виртуализатором только для видимых строк при скролле
    private void OnRowContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        if (e.Container.DataContext is List<ImageItem> rowItems)
        {
            foreach (var item in rowItems)
            {
                //item.EnsureLoaded();
                item.LoadThumbnailAsync();
            }
        }
    }
    private void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
        if (e.Container.DataContext is List<ImageItem> rowItems)
        {
            foreach (var item in rowItems)
            {
                /*(if (item.Thumbnail != null)
                {
                    var old = item.Thumbnail;
                    item.Thumbnail = null;
                    old.Dispose();
                }*/
                item.UnloadThumbnail();
            }
        }
    }
    // Автоматический подбор числа колонок под ширину окна (ширина плитки ~180px)
    private void OnListSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            int cols = (int)(e.NewSize.Width / (PreviewWidth + 10));
            vm.UpdateColumns(cols);
        }
    }

    // Клик по конкретной плитке
    private void OnTilePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control &&
            control.DataContext is ImageItem item &&
            DataContext is MainViewModel vm)
        {
            var properties = e.GetCurrentPoint(this).Properties;

            if (properties.IsLeftButtonPressed)
            {
                bool isCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
                bool isShift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
                vm.HandleSelection(item, isCtrl, isShift);
            }
            else if (properties.IsRightButtonPressed)
            {
                // Если нажали ПКМ по невыделенной плитке — выделяем её.
                // Если она уже входит в группу выделенных — сохраняем выделение всей группы.
                vm.SelectOnRightClick(item);
            }
        }
    }
    private async void OnTileDoubleTapped(object? sender, TappedEventArgs e)
    {
        var win = new ViewImageWindow()
        {
            DataContext = this.DataContext
        };
        await win.ShowDialog(this);
        if (win.SelectedTags.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < win.SelectedTags.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(win.SelectedTags[i]);
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(win.SelectedTags[i]);
                }
            }
            autobox.Text = sb.ToString();
            option_comboBox.SelectedIndex = 0;
            SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        else
        {
            MainViewModel vm = (MainViewModel)DataContext;
            int result = (int)Math.Ceiling((double)vm.CountImages / (double)vm.ColumnsCount);
            RowsListBox.ScrollIntoView(result);
        }
    }
    private async void OnOpenMenuItemClick(object? sender, RoutedEventArgs e)
    {
        var win = new ViewImageWindow()
        {
            DataContext = this.DataContext
        };
        await win.ShowDialog(this);
        if (win.SelectedTags.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < win.SelectedTags.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(win.SelectedTags[i]);
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(win.SelectedTags[i]);
                }
            }
            autobox.Text = sb.ToString();
            SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        else
        {
            MainViewModel vm = (MainViewModel)DataContext;
            int result = (int)Math.Ceiling((double)vm.CountImages / (double)vm.ColumnsCount);
            RowsListBox.ScrollIntoView(result);
        }
    }
    private async void OnOpenInFullScreenMenuItemClick(object? sender, RoutedEventArgs e)
    {
        var win = new FullScreenWindow()
        {
            DataContext = this.DataContext
        };
        await win.ShowDialog(this);
        MainViewModel vm = (MainViewModel)DataContext;
        int result = (int)Math.Ceiling((double)vm.CountImages / (double)vm.ColumnsCount);
        RowsListBox.ScrollIntoView(result);
    }
    private async void OnRemoveMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var toRemove = vm._allImages.Where(x => x.IsSelected).ToList();
        if (toRemove.Count == 0) return;
        var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.YesNo,
            ContentTitle = "Подтверждение",
            ContentMessage = "Изображений выбрано: " + toRemove.Count + "\nУдалить?",
            Icon = MsBox.Avalonia.Enums.Icon.Question,
            // Открыть строго по центру родительского окна
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        });
        // Обязательно передаем 'this' (ссылку на родительский Window)
        var result = await box.ShowWindowDialogAsync(this);
        if (result == ButtonResult.Yes)
        {
            vm.RemoveSelected(toRemove);
            ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
        }
    }
    private async void OnOpenSystemAppMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null && File.Exists(first.FilePath))
        {
            Process.Start(new ProcessStartInfo(first.FilePath) { UseShellExecute = true });
        }
    }
    private async void OnShowInExplorerMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null && File.Exists(first.FilePath))
        {
            // Открывает папку и выделяет файл в Проводнике Windows
            Process.Start("explorer.exe", $"/select,\"{first.FilePath}\"");
        }
    }
    private async void CopyToWallMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var toCopy = vm._allImages.Where(x => x.IsSelected).ToList();
        if (toCopy.Count == 0) return;
        foreach (var item in toCopy)
        {
            try
            {
                string dest_path = WallPath + Path.GetFileName(item.ImageInfo.FilePath);
                if (File.Exists(dest_path))
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.YesNo,
                        ContentTitle = "Подтверждение",
                        ContentMessage = "Целевой фаил уже сушествует, перезаписать?",
                        Icon = MsBox.Avalonia.Enums.Icon.Warning,
                        // Открыть строго по центру родительского окна
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        EnterDefaultButton = ClickEnum.Ok
                    });
                    // Обязательно передаем 'this' (ссылку на родительский Window)
                    var result = await box.ShowWindowDialogAsync(this);
                    if (result == ButtonResult.Yes)
                    {
                        File.Delete(dest_path);
                        File.Copy(item.ImageInfo.FilePath, dest_path);
                    }
                }
                else
                {
                    File.Copy(item.ImageInfo.FilePath, dest_path);
                }
            }
            catch (IOException ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Ошибка!",
                    ContentMessage = ex.Message,
                    Icon = MsBox.Avalonia.Enums.Icon.Error,
                    // Открыть строго по центру родительского окна
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                // Обязательно передаем 'this' (ссылку на родительский Window)
                await box.ShowWindowDialogAsync(this);
            }
        }
    }
    private async void CopyHashToClipboardMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard is not null)
            {
                await clipboard.SetTextAsync(first.ImageInfo.Hash);
            }
        }
    }
    private async void CopyToDirMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var toCopy = vm._allImages.Where(x => x.IsSelected).ToList();
        if (toCopy.Count == 0) return;
        // Получаем TopLevel от текущего окна или контрола
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        // Открываем системный диалог выбора папки
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку",
            AllowMultiple = false // true, если нужно выбрать несколько папок
        });

        if (folders.Count > 0)
        {
            // Получаем локальный путь к папке (на десктопе)
            string? folderPath = folders[0].TryGetLocalPath();

            // Или URI (универсально для всех платформ, включая мобильные и Web)
            //Uri folderUri = folders[0].Path;

            foreach (var item in toCopy)
            {
                try
                {
                    string dest_path = folderPath + "\\" + Path.GetFileName(item.ImageInfo.FilePath);
                    if (File.Exists(dest_path))
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                        {
                            ButtonDefinitions = ButtonEnum.YesNo,
                            ContentTitle = "Подтверждение",
                            ContentMessage = "Целевой фаил уже сушествует, перезаписать?",
                            Icon = MsBox.Avalonia.Enums.Icon.Warning,
                            // Открыть строго по центру родительского окна
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            EnterDefaultButton = ClickEnum.Ok
                        });
                        // Обязательно передаем 'this' (ссылку на родительский Window)
                        var result = await box.ShowWindowDialogAsync(this);
                        if (result == ButtonResult.Yes)
                        {
                            File.Delete(dest_path);
                            File.Copy(item.ImageInfo.FilePath, dest_path);
                        }
                    }
                    else
                    {
                        File.Copy(item.ImageInfo.FilePath, dest_path);
                    }
                }
                catch (IOException ex)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.Ok,
                        ContentTitle = "Ошибка!",
                        ContentMessage = ex.Message,
                        Icon = MsBox.Avalonia.Enums.Icon.Error,
                        // Открыть строго по центру родительского окна
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    });
                    // Обязательно передаем 'this' (ссылку на родительский Window)
                    await box.ShowWindowDialogAsync(this);
                }
            }
        }
    }
    private async void MoveToDirMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var toCopy = vm._allImages.Where(x => x.IsSelected).ToList();
        if (toCopy.Count == 0) return;
        // Получаем TopLevel от текущего окна или контрола
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        // Открываем системный диалог выбора папки
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку",
            AllowMultiple = false // true, если нужно выбрать несколько папок
        });

        if (folders.Count > 0)
        {
            // Получаем локальный путь к папке (на десктопе)
            string? folderPath = folders[0].TryGetLocalPath();

            // Или URI (универсально для всех платформ, включая мобильные и Web)
            //Uri folderUri = folders[0].Path;

            foreach (var item in toCopy)
            {
                try
                {
                    string dest_path = folderPath + "\\" + Path.GetFileName(item.ImageInfo.FilePath);
                    if (File.Exists(dest_path))
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                        {
                            ButtonDefinitions = ButtonEnum.YesNo,
                            ContentTitle = "Подтверждение",
                            ContentMessage = "Целевой фаил уже сушествует, перезаписать?",
                            Icon = MsBox.Avalonia.Enums.Icon.Warning,
                            // Открыть строго по центру родительского окна
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            EnterDefaultButton = ClickEnum.Ok
                        });
                        // Обязательно передаем 'this' (ссылку на родительский Window)
                        var result = await box.ShowWindowDialogAsync(this);
                        if (result == ButtonResult.Yes)
                        {
                            File.Delete(dest_path);
                            File.Move(item.ImageInfo.FilePath, dest_path);
                        }
                    }
                    else
                    {
                        File.Move(item.ImageInfo.FilePath, dest_path);
                    }
                }
                catch (IOException ex)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.Ok,
                        ContentTitle = "Ошибка!",
                        ContentMessage = ex.Message,
                        Icon = MsBox.Avalonia.Enums.Icon.Error,
                        // Открыть строго по центру родительского окна
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    });
                    // Обязательно передаем 'this' (ссылку на родительский Window)
                    await box.ShowWindowDialogAsync(this);
                }
            }
        }
    }
    private async void OnCopyTagsToClipboardMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null && File.Exists(first.FilePath))
        {
            List<string> tags = ErzaDB.GetTagsByImageIDToString(first.ImageInfo.ImageID, MainWindow.Erza);
            if (tags.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tags.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(tags[i]);
                    }
                    else
                    {
                        sb.Append(' ');
                        sb.Append(tags[i]);
                    }
                }
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard is not null)
                {
                    await clipboard.SetTextAsync(sb.ToString());
                }
            }
        }
    }
    private async void OnNoTagsShowMenuItemClick(object? sender, RoutedEventArgs e)
    {
        autobox.Text = String.Empty;
        MainViewModel vm = (MainViewModel)DataContext;
        vm.LoadImagesByNoTags();
        ResultText.Text = vm.CountImages.ToString();
    }
    private async void OnFavoritedShowMenuItemClick(object? sender, RoutedEventArgs e)
    {
        autobox.Text = String.Empty;
        MainViewModel vm = (MainViewModel)DataContext;
        vm.LoadImagesByFavorited();
        ResultText.Text = vm.CountImages.ToString();
    }
    private async void OnAddTagsMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null)
        {
            var dialog = new AddTagsWindow();
            // Ждём закрытия и получаем результат (если закрыли крестиком — вернётся null)
            string? result = await dialog.ShowDialog<string>(this);
            if (result != null)
            {
                ImageInfo img = first.ImageInfo;
                string[] tags = result.Trim().Split(' ');
                img.AddTags(tags);
                ErzaDB.LoadImageToErza(img, MainWindow.Erza);
            }
        }
    }
    private async void OnAddToFavoritedMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null)
        {
            ErzaDB.SetImageFavorit(first.ImageInfo.ImageID, true, MainWindow.Erza);
            first.ImageInfo.Favorited = true;
        }
    }
    private async void OnRemoveToFavoritedMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null)
        {
            ErzaDB.SetImageFavorit(first.ImageInfo.ImageID, false, MainWindow.Erza);
            first.ImageInfo.Favorited = false;
        }
    }
    private async void OnReCreateThumbnailMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var toCopy = vm._allImages.Where(x => x.IsSelected).ToList();
        //if (toCopy.Count == 0) return;
        foreach (var item in toCopy)
        {
            try
            {
                var p_size = new MagickGeometry(PreviewWidth, PreviewHeight);
                string dest_file = PreviewPath + "\\" + item.ImageInfo.Hash[0] + "\\" + item.ImageInfo.Hash[1] + "\\" + item.ImageInfo.Hash + ".jpg";
                using (MagickImage img = new MagickImage(item.ImageInfo.FilePath))
                {
                    img.Resize(p_size);
                    img.Format = MagickFormat.Jpeg;
                    img.SetCompression(CompressionMethod.JPEG);
                    img.Quality = 80;
                    img.Write(dest_file);
                }
                item.LoadThumbnailAsync();
            }
            catch (IOException ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Ошибка!",
                    ContentMessage = ex.Message,
                    Icon = MsBox.Avalonia.Enums.Icon.Error,
                    // Открыть строго по центру родительского окна
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                // Обязательно передаем 'this' (ссылку на родительский Window)
                await box.ShowWindowDialogAsync(this);
            }
        }
    }
    private async void OnFindSimilarMenuItemClick(object? sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        var first = vm._allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null)
        {
            if (first.ImageInfo.PHash == null)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Ошибка!",
                    ContentMessage = "Нет PHash",
                    Icon = MsBox.Avalonia.Enums.Icon.Error,
                    // Открыть строго по центру родительского окна
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                // Обязательно передаем 'this' (ссылку на родительский Window)
                await box.ShowWindowDialogAsync(this);
                return;
            }
            var dialog = new FindSimilarOptionsWindow();
            // Ждём закрытия и получаем результат (если закрыли крестиком — вернётся null)
            decimal? result = await dialog.ShowDialog<decimal>(this);
            if (result != null)
            {
                double similar = Convert.ToDouble(result);
                vm.LoadImagesBySimillar(first, similar);
            }
        }
    }
    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        Erza.Close();
    }
    private void SearchButtonClick(object? sender, RoutedEventArgs e)
    {
        if (autobox.Text == null || autobox.Text.Length == 0)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadAllImages();
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsTags())
        {
            string[] temp = this.autobox.Text.Split(' ');
            List<string> tags = new List<string>();
            foreach (string tag in temp)
            {
                if (!String.IsNullOrEmpty(tag))
                {
                    tags.Add(tag);
                }
            }
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByTags(tags);
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsTagsOr())
        {
            string[] temp = this.autobox.Text.Split(' ');
            List<string> tags = new List<string>();
            foreach (string tag in temp)
            {
                if (!String.IsNullOrEmpty(tag))
                {
                    tags.Add(tag);
                }
            }
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByTagsOr(tags);
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsPartTag())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByPartTag(this.autobox.Text);
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsMD5())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByMD5(this.autobox.Text);
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsRatioWnH())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByRatioWnH(this.autobox.Text);
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsSQL())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesBySQL(this.autobox.Text);
                ResultText.Text = vm.CountImages.ToString();
            }
            return;
        }
    }
    private async void SlideShowButtonClick(object? sender, RoutedEventArgs e)
    {
        var win = new SlideShowWindow()
        {
            AllImages = ((MainViewModel)DataContext)._allImages,
            Delay = 5
        };
        await win.ShowDialog(this);
    }
    private bool SelectIsTags()
    {
        if (this.option_comboBox.Text == "Теги")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool SelectIsTagsOr()
    {
        if (this.option_comboBox.Text == "Теги ИЛИ")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool SelectIsPartTag()
    {
        if (this.option_comboBox.Text == "Часть тега")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool SelectIsMD5()
    {
        if (this.option_comboBox.Text == "MD5")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool SelectIsRatioWnH()
    {
        if (this.option_comboBox.Text == "Соотношение ширины и высоты")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool SelectIsSQL()
    {
        if (this.option_comboBox.Text == "SQL")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private string GetLastWord(string text)
    {
        int lastSpaceIndex = text.LastIndexOf(' ');
        return lastSpaceIndex == -1 ? text : text.Substring(lastSpaceIndex + 1);
    }
}
