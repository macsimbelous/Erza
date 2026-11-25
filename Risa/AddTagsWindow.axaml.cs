using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ErzaLib2;
using HarfBuzzSharp;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Risa
{
    public partial class AddTagsWindow : Window
    {
        public ObservableCollection<string> Tags { get; } = new();
        public AddTagsWindow()
        {
            InitializeComponent();
            using (SQLiteCommand command = new SQLiteCommand(MainWindow.Erza))
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
        private string GetLastWord(string text)
        {
            int lastSpaceIndex = text.LastIndexOf(' ');
            return lastSpaceIndex == -1 ? text : text.Substring(lastSpaceIndex + 1);
        }
        // Нажатие на кнопку "Сохранить"
        private void OnOKClick(object? sender, RoutedEventArgs e)
        {
            Close(autobox.Text); // Закрываем окно и возвращаем объект
        }

        // Нажатие на кнопку "Отмена"
        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close(null); // Закрываем и возвращаем null
        }
        private void OnActionButtonClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string tag })
            {
                autobox.Text = autobox.Text + " " + tag;
            }
        }
    }
}