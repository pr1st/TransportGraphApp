using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class GraphParametersDialog : Window {

        private PositiveDoubleRowControl _circleRadiusControl;
        private PositiveDoubleRowControl _edgeThicknesControl;
        private GraphConfig _oldConfig;
        
        public GraphParametersDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            Icon = AppResources.GetAppIcon;

            _oldConfig = AppGraph.Instance.GraphConfig;
            _circleRadiusControl = new PositiveDoubleRowControl() {
                TitleValue = "Радиус круга нас. пункта (в пикселях)",
                Value = _oldConfig.CircleRadius
            };
            _edgeThicknesControl = new PositiveDoubleRowControl() {
                TitleValue = "Толшина прямой маршрута (в пикселях)",
                Value = _oldConfig.EdgeThickness
            };

            PropertiesPanel.Children.Add(_circleRadiusControl);
            PropertiesPanel.Children.Add(_edgeThicknesControl);
        }
        
        private void ApplyClick(object sender, RoutedEventArgs e) {
            _oldConfig.CircleRadius = _circleRadiusControl.Value;
            _oldConfig.EdgeThickness = _edgeThicknesControl.Value;
            AppGraph.Instance.GraphConfig = _oldConfig;
            AppGraph.Instance.DrawGraph();
            DialogResult = true;
        }
    }
}