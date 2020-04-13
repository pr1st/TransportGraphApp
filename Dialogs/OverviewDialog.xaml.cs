using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class OverviewDialog : Window {
        public OverviewDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
        }
    }
}