using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Risa
{
    public partial class FindSimilarOptionsWindow : Window
    {
        public FindSimilarOptionsWindow()
        {
            InitializeComponent();
        }
        // Нажатие на кнопку "Сохранить"
        private void OnOKClick(object? sender, RoutedEventArgs e)
        {
            Close(SimilarNumericUpDown.Value); // Закрываем окно и возвращаем объект
        }

        // Нажатие на кнопку "Отмена"
        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close(null); // Закрываем и возвращаем null
        }
    }
}