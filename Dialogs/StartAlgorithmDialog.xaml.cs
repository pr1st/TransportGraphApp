using System;
using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class StartAlgorithmDialog : Window {
        private AlgorithmConfigControl _algorithmConfigControl;
        
        public StartAlgorithmDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            Icon = AppResources.GetAppIcon;

            _algorithmConfigControl = new AlgorithmConfigControl();
            PropertiesPanel.Children.Add(_algorithmConfigControl);
        }
        
        private void StartClick(object sender, RoutedEventArgs e) {
            AppAlgorithm.Instance.StartAlgorithm(_algorithmConfigControl.AlgorithmConfig);
            // todo show result window
            DialogResult = true;
        }
    }
}