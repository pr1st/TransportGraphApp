using System.IO;
using System.Windows;

namespace TransportGraphApp.Dialogs {
    public partial class NewDataBaseFileDialog : Window {
        public NewDataBaseFileDialog() {
            InitializeComponent();
            Icon = AppResources.GetAppIcon;
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            if (File.Exists(NewDataBaseFileName)) {
                ComponentUtils.ShowMessage("This file already exists", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        public string NewDataBaseFileName => $"{FileName.Value}.db";
    }
}