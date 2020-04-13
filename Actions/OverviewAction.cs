using System.Windows;

namespace TransportGraphApp.Actions {
    public static class OverviewAction {
        public static void Invoke() {
            // new OverviewDialog().ShowDialog();
            ComponentUtils.ShowMessage("Скоро будет...", MessageBoxImage.Information);
        }
    }
}