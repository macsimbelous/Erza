using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErzaLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
namespace Risa;

public partial class MainViewModel : ObservableObject
{
    private readonly List<ImageItem> _allImages = new();
    public int CountImages { get {  return _allImages.Count; }  }

    // Опорный элемент для выделения диапазона через Shift
    private ImageItem? _anchorItem;

    public ObservableCollection<List<ImageItem>> ImageRows { get; } = new();

    [ObservableProperty]
    private int _selectedCount;

    [ObservableProperty]
    private int _columnsCount = 4;

    public MainViewModel()
    {
        /*string folder = @"D:\previews";

        if (Directory.Exists(folder))
        {
            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                _allImages.Add(new ImageItem(file, Path.GetFileName(file)));
            }
        }*/
        //LoadAllImages();
    }
    public void LoadAllImages()
    {
        List<ImageInfo> Result = ErzaDB.GetAllImages(false, MainWindow.Erza);
        _allImages.Clear();
        foreach (ImageInfo img in Result)
        {
            //imageListView2.Items.Add(img, img.Hash, adaptor);
            _allImages.Add(new ImageItem(img));
        }
        //this.ResultText.Text = "Изображений найдено: " + Result.Count.ToString();
        RebuildRows();
    }
    public void LoadImagesByTags(List<string> Tags)
    {
        _allImages.Clear();
        if (Tags.Count > 1)
        {
            List<ImageInfo> Result = ErzaDB.GetImagesByTags(Tags, false, MainWindow.Erza);
            foreach (ImageInfo img in Result)
            {
                _allImages.Add(new ImageItem(img));
            }
        }
        else
        {
            List<ImageInfo> Result;
            if (Tags.Count == 1)
            {
                Result = ErzaDB.GetImagesByTag(Tags[0], MainWindow.Erza);
            }
            else
            {
                Result = ErzaDB.GetAllImages(false, MainWindow.Erza);
            }
            foreach (ImageInfo img in Result)
            {
                _allImages.Add(new ImageItem(img));
            }
        }
        RebuildRows();
    }
    public void LoadImagesByTagsOr(List<string> Tags)
    {
        if (Tags.Count > 1)
        {
            List<ImageInfo> Result = ErzaDB.GetImagesByTags(Tags, true, MainWindow.Erza);
            _allImages.Clear();
            foreach (ImageInfo img in Result)
            {
                _allImages.Add(new ImageItem(img));
            }
        }
        else
        {
            List<ImageInfo> Result;
            if (Tags.Count == 1)
            {
                Result = ErzaDB.GetImagesByTag(Tags[0], MainWindow.Erza);
            }
            else
            {
                Result = ErzaDB.GetAllImages(false, MainWindow.Erza);
            }
            _allImages.Clear();
            foreach (ImageInfo img in Result)
            {
                _allImages.Add(new ImageItem(img));
            }
        }
        RebuildRows();
    }
    public void LoadImagesByPartTag(string PartTag)
    {
        List<ImageInfo> Result = ErzaDB.GetImagesByPartTag(PartTag, MainWindow.Erza);
        _allImages.Clear();
        foreach (ImageInfo img in Result)
        {
            _allImages.Add(new ImageItem(img));
        }
        RebuildRows();
    }
    public void LoadImagesByMD5(string MD5)
    {
        _allImages.Clear();
        ImageInfo img = ErzaDB.GetImageWithOutTags(MD5, MainWindow.Erza);
        if (img != null)
        {
            _allImages.Add(new ImageItem(img));
        }
        RebuildRows();
    }
    public void LoadImagesByRatioWnH(string Ratio)
    {
        _allImages.Clear();
        List<ImageInfo> Result = new List<ImageInfo>();
        string sql = "SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL AND CAST(width AS FLOAT) / CAST(height AS FLOAT) " + Ratio;
        try
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, MainWindow.Erza))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Hash = (string)reader["hash"];
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
                    }
                    Result.Add(image);
                }
                reader.Close();
            }
            foreach (ImageInfo img in Result)
            {
                _allImages.Add(new ImageItem(img));
            }
        }
        catch (Exception ex)
        {
            //MessageBox.Show(ex.Message);
        }
        RebuildRows();
    }
    public void LoadImagesBySQL(string SQL)
    {
        _allImages.Clear();
        List<ImageInfo> Result = new List<ImageInfo>();
        string sql = "SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL AND " + SQL;
        try
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, MainWindow.Erza))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Hash = (string)reader["hash"];
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
                    }
                    Result.Add(image);
                }
                reader.Close();
            }
            foreach (ImageInfo img in Result)
            {
                _allImages.Add(new ImageItem(img));
            }
        }
        catch (Exception ex)
        {
            //MessageBox.Show(ex.Message);
        }
        RebuildRows();
    }
    public void UpdateColumns(int newColumns)
    {
        if (newColumns < 1) newColumns = 1;
        if (ColumnsCount == newColumns && ImageRows.Count > 0) return;

        ColumnsCount = newColumns;
        RebuildRows();
    }

    private void RebuildRows()
    {
        ImageRows.Clear();
        foreach (var chunk in _allImages.Chunk(ColumnsCount))
        {
            ImageRows.Add(chunk.ToList());
        }
    }

    // Логика выделения: обычный клик, Ctrl+клик, Shift+клик, Ctrl+Shift+клик
    public void HandleSelection(ImageItem clickedItem, bool isCtrlPressed, bool isShiftPressed)
    {
        if (isShiftPressed && _anchorItem != null)
        {
            int anchorIndex = _allImages.IndexOf(_anchorItem);
            int currentIndex = _allImages.IndexOf(clickedItem);

            if (anchorIndex != -1 && currentIndex != -1)
            {
                int start = Math.Min(anchorIndex, currentIndex);
                int end = Math.Max(anchorIndex, currentIndex);

                // Если Ctrl НЕ зажат, сбрасываем выделение вне нового диапазона
                if (!isCtrlPressed)
                {
                    for (int i = 0; i < _allImages.Count; i++)
                    {
                        _allImages[i].IsSelected = (i >= start && i <= end);
                    }
                }
                else
                {
                    // Если зажат Ctrl + Shift, добавляем диапазон к уже выделенным
                    for (int i = start; i <= end; i++)
                    {
                        _allImages[i].IsSelected = true;
                    }
                }
            }
        }
        else if (isCtrlPressed)
        {
            // Инвертируем состояние конкретного элемента и обновляем якорь
            clickedItem.IsSelected = !clickedItem.IsSelected;
            _anchorItem = clickedItem;
        }
        else
        {
            // Одиночный клик: снимаем выделение со всех и выделяем текущий
            foreach (var item in _allImages)
            {
                item.IsSelected = (item == clickedItem);
            }
            _anchorItem = clickedItem;
        }

        SelectedCount = _allImages.Count(x => x.IsSelected);
    }
    public void OpenImage(ImageItem item)
    {
        if (!File.Exists(item.FilePath)) return;

        // Пример: открыть изображение в стандартном просмотрщике системы
        Process.Start(new ProcessStartInfo(item.FilePath)
        {
            UseShellExecute = true
        });
    }
    // Правый клик: если плитка еще не выделена, выделяем только её
    public void SelectOnRightClick(ImageItem clickedItem)
    {
        if (!clickedItem.IsSelected)
        {
            HandleSelection(clickedItem, isCtrlPressed: false, isShiftPressed: false);
        }
    }

    [RelayCommand]
    private void OpenSelected()
    {
        var first = _allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null && File.Exists(first.FilePath))
        {
            Process.Start(new ProcessStartInfo(first.FilePath) { UseShellExecute = true });
        }
    }

    [RelayCommand]
    private void ShowInExplorer()
    {
        var first = _allImages.FirstOrDefault(x => x.IsSelected);
        if (first != null && File.Exists(first.FilePath))
        {
            // Открывает папку и выделяет файл в Проводнике Windows
            Process.Start("explorer.exe", $"/select,\"{first.FilePath}\"");
        }
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        var toRemove = _allImages.Where(x => x.IsSelected).ToList();
        if (toRemove.Count == 0) return;

        foreach (var item in toRemove)
        {
            item.Thumbnail?.Dispose(); // Освобождаем память
            _allImages.Remove(item);
        }

        SelectedCount = 0;
        RebuildRows(); // Перестраиваем сетку без удаленных элементов
    }
}