using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TransportGraphApp {
    internal static class ComponentUtils {
        public static void ShowMessage(string message, MessageBoxImage icon) {
            MessageBox.Show(
                message,
                AppResources.GetAppTitle,
                MessageBoxButton.OK,
                icon
            );
        }

        public static TextBox DoubleTextBox() {
            var numBox = new TextBox() {
                FontSize = 16, 
                MinWidth = 120};
            numBox.PreviewTextInput += NumberValidationTextBox;
            return numBox;
        }

        public static Button ButtonWithIcon(ImageSource icon) {
            var img = new Image { Source = icon };

            // without this do not work :(
            Console.WriteLine(img.Source.Height);

            var stackPnl = new StackPanel { Orientation = Orientation.Horizontal };
            stackPnl.Children.Add(img);

            var button = new Button { Content = stackPnl, Margin = new Thickness(5, 0, 5, 0) };
            return button;
        }

        private static void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var initial = ((TextBox) sender).Text;
            var received = e.Text;
            var caretIndex = ((TextBox) sender).CaretIndex;
            string result;
            if (initial.Length > 0) {
                result = initial.Substring(0, caretIndex) + received +
                         initial.Substring(caretIndex, initial.Length - caretIndex);
            }
            else {
                result = received;
            }

            e.Handled = !double.TryParse(result, out _);
        }
    }
}