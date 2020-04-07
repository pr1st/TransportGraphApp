using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class GraphParametersDialog : Window {
        public GraphParametersDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            Icon = AppResources.GetAppIcon;
        }
    }
}