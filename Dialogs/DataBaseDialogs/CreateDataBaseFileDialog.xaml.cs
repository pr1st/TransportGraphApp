using System.IO;
using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs.DataBaseDialogs {
    public partial class CreateDataBaseFileDialog : Window {
        public CreateDataBaseFileDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            Icon = AppResources.GetAppIcon;
        }

        public string NewDataBaseFileName => $"{FileNameBox.Text}.db";

        public bool OpenAfterCreation { get; set; }

        private void CreateAndOpenClicked(object sender, RoutedEventArgs e) {
            if (File.Exists(NewDataBaseFileName)) {
                ComponentUtils.ShowMessage("Файл с таким именем уже существует", MessageBoxImage.Error);
                return;
            }

            OpenAfterCreation = true;
            DialogResult = true;
        }

        private void CreateClicked(object sender, RoutedEventArgs e) {
            if (File.Exists(NewDataBaseFileName)) {
                ComponentUtils.ShowMessage("Файл с таким именем уже существует", MessageBoxImage.Error);
                return;
            }

            OpenAfterCreation = false;
            DialogResult = true;
        }
    }
}