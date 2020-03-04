using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class ChangeGraphAttributesDialog : Window {
        public Graph UpdatedGraph => new Graph() {
            Id = _initGraph.Id,
            Name = _initGraph.Name,
            GraphAttributes = _changeBox.UpdatedAttributes,
            DefaultNodeAttributes = _initGraph.DefaultNodeAttributes,
            DefaultEdgeAttributes = _initGraph.DefaultEdgeAttributes
        };

        private readonly Graph _initGraph;
        private readonly AttributesChangeBox _changeBox;

        public ChangeGraphAttributesDialog(Graph graph) {
            _initGraph = graph;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;

            GraphNameLabel.Content = graph.Name;
            _changeBox = new AttributesChangeBox(graph.GraphAttributes);
            AttributePanel.Children.Add(_changeBox);
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}