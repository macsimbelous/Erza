using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ErzaLib2;
using HarfBuzzSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Risa;

public partial class MainWindow : Window
{
    public static SQLiteConnection Erza;
    public static string PreviewPath = "D:\\previews";
    private const int PreviewWidth = 300;
    private const int PreviewHeight = 225;
    public ObservableCollection<string> Tags { get; } = new()
    {
        "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Иркутск"
    };
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
    }
    // Вызывается виртуализатором только для видимых строк при скролле
    private void OnRowContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        if (e.Container.DataContext is List<ImageItem> rowItems)
        {
            foreach (var item in rowItems)
            {
                item.EnsureLoaded();
            }
        }
    }
    private void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
        if (e.Container.DataContext is List<ImageItem> rowItems)
        {
            foreach (var item in rowItems)
            {
                if (item.Thumbnail != null)
                {
                    var old = item.Thumbnail;
                    item.Thumbnail = null;
                    old.Dispose();
                }
            }
        }
    }
    // Автоматический подбор числа колонок под ширину окна (ширина плитки ~180px)
    private void OnListSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            int cols = (int)(e.NewSize.Width / PreviewWidth);
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
    private void OnTileDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control control &&
            control.DataContext is ImageItem item &&
            DataContext is MainViewModel vm)
        {
            vm.OpenImage(item);
            e.Handled = true;
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
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
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
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
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
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsPartTag())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByPartTag(this.autobox.Text);
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsMD5())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByMD5(this.autobox.Text);
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsRatioWnH())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesByRatioWnH(this.autobox.Text);
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
            }
            return;
        }
        if (SelectIsSQL())
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadImagesBySQL(this.autobox.Text);
                ResultText.Text = "Изображений найдено: " + vm.CountImages.ToString();
            }
            return;
        }
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
}