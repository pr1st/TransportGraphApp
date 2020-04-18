using System.Windows;

namespace TransportGraphApp.Dialogs {
    public partial class TaskSpecificationDialog : Window {
        public TaskSpecificationDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            
            Closed += (sender, args) => CancelClick();
        }
        
        private void RunClick(object sender, RoutedEventArgs e) {
            
        }
        
        private void CheckClick(object sender, RoutedEventArgs e) {
            
        }

        private void CancelClick() {
            
        }
    }
}