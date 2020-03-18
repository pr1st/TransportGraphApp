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
    }
}