using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportGraphApp.Models;

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

        public static void InsertIconToButton(Button button, ImageSource icon, string toolTip) {
            button.Background = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);
            button.Height = 24;
            button.Width = 24;
            button.ToolTip = toolTip;
            
            var img = new Image { Source = icon };

            // without this do not work :(
            Console.WriteLine(img.Source.Height);

            var stackPnl = new StackPanel { Orientation = Orientation.Horizontal };
            stackPnl.Children.Add(img);

            button.Content = stackPnl;
        }
    }
}