using System.Windows;

namespace TransportGraphApp.Actions {
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
