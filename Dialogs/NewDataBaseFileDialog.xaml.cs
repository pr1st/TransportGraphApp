using System.IO;
using System.Windows;
using TransportGraphApp.Actions;

namespace TransportGraphApp.Dialogs {
    public partial class NewDataBaseFileDialog : Window {
        public NewDataBaseFileDialog() {
            InitializeComponent();
            Icon = AppResources.GetAppIcon;
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            var path = $"{FileName.Text}.db";
            if (File.Exists(path)) {
                ComponentUtils.ShowMessage("This file already exists", MessageBoxImage.Error);
            } else {
                DialogResult = true;
            }
        }
    }
}